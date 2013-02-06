DROP VIEW ListaPecas

DROP PROCEDURE RemoveCliente
DROP PROCEDURE AlteraCliente
DROP PROCEDURE InsereCliente
DROP PROCEDURE InserirPeca
DROP PROCEDURE AlterarPrecoPeca

DROP TRIGGER RegistaHistoricoPeca 
DROP TRIGGER ValidaProfissaoFuncionarioDepartamento 
DROP TRIGGER ValidaFuncionarioObra 
DROP TRIGGER ValidaClienteObra 
DROP TRIGGER ImpedeRemocaoFuncionario 
DROP TRIGGER ImpedeRemocaoObra 
GO

CREATE VIEW ListaPecas
AS
	SELECT p.refP, p.designacaoP, p.precoP, COUNT(r.obra) as nobras FROM Peca p
	JOIN Reserva r ON p.refP=r.peca
	GROUP BY p.refP, p.designacaoP, p.precoP
GO

/* Não é possível eliminar qualquer obra do sistema */
CREATE TRIGGER ImpedeRemocaoObra ON Obra INSTEAD OF DELETE
AS BEGIN
	ROLLBACK
END
GO

/* Nunca pode ser removido um registo de funcionário */
CREATE TRIGGER ImpedeRemocaoFuncionario ON Funcionario INSTEAD OF DELETE
AS BEGIN
	ROLLBACK
END
GO

/* Uma pessoa não pode ser, simultaneamente, cliente e funcionário na mesma obra */

CREATE TRIGGER ValidaClienteObra ON Factura FOR INSERT, UPDATE
AS BEGIN
	DECLARE @cliente INT
	DECLARE @obra INT
	DECLARE @oficina INT
	DECLARE clicur CURSOR FOR SELECT cliente, obra, oficina FROM inserted
	OPEN clicur
	FETCH NEXT FROM clicur INTO @cliente, @obra, @oficina
	WHILE (@@FETCH_STATUS=0) BEGIN
		IF (EXISTS (SELECT funcionario FROM ObraContem WHERE funcionario=@cliente AND obra=@obra and oficina=@oficina)) BEGIN
			ROLLBACK
			BREAK
		END
		FETCH NEXT FROM clicur INTO @cliente, @obra, @oficina
	END
	CLOSE clicur
	DEALLOCATE clicur
END
GO

CREATE TRIGGER ValidaFuncionarioObra ON ObraContem FOR INSERT, UPDATE
AS BEGIN
	DECLARE @func INT
	DECLARE @obra INT
	DECLARE @oficina INT
	DECLARE funccur CURSOR FOR SELECT funcionario, obra, oficina FROM inserted
	OPEN funccur
	FETCH NEXT FROM funccur INTO @func, @obra, @oficina
	WHILE (@@FETCH_STATUS=0) BEGIN
		IF (EXISTS (SELECT cliente FROM Factura WHERE cliente=@func AND obra=@obra and oficina=@oficina)) BEGIN
			ROLLBACK
			BREAK
		END
		FETCH NEXT FROM funccur INTO @func, @obra, @oficina
	END
	CLOSE funccur
	DEALLOCATE funccur
END
GO

/* Num departamento só podem estar associados funcionários cuja profissão faça sentido nesse departamento */
CREATE TRIGGER ValidaProfissaoFuncionarioDepartamento ON FuncionarioDepartamento FOR INSERT, UPDATE
AS BEGIN
	DECLARE @prof INT
	DECLARE @dep INT
	DECLARE @ofic INT
	DECLARE profcur CURSOR FOR SELECT f.profissao, departamento, oficina FROM inserted i 
		JOIN Funcionario f ON f.codFunc=i.funcionario
	OPEN profcur
	FETCH NEXT FROM profcur INTO @prof, @dep, @ofic
	WHILE (@@FETCH_STATUS=0) BEGIN
		IF (NOT EXISTS (SELECT profissao FROM ProfissaoDepartamento 
				WHERE profissao=@prof AND departamento=@dep and oficina=@ofic)) BEGIN
			ROLLBACK
			BREAK
		END
		FETCH NEXT FROM profcur INTO @prof, @dep, @ofic
	END
	CLOSE profcur
	DEALLOCATE profcur
END
GO

/* Sempre que existir alteração ao preço de uma peça, deve ser de imediato registado numa tabela de histórico o preço
antigo e a data até à qual esteve em vigor */
CREATE TRIGGER RegistaHistoricoPeca ON Peca FOR UPDATE
AS BEGIN
	IF (UPDATE(precoP)) BEGIN
		INSERT INTO HistoricoPecas (refP, designacaoP, precoP, dataP)
		SELECT i.refP, i.designacaoP, i.precoP, GETDATE() FROM inserted i
		JOIN deleted d ON i.refP=d.refP 
		WHERE i.precoP!=d.precoP
	END
END
GO

CREATE PROCEDURE InsereCliente(@nif INT, @nome NOME_CLIENTE, @morada MORADA_CLIENTE, 
							   @telefone TELEFONE_CLIENTE, @email EMAIL_CLIENTE, @tipo TIPO_CLIENTE)
AS BEGIN
	IF (@nome = '' OR @tipo = '')
		RETURN -1
	SET @tipo = LOWER(@tipo)
	IF (@tipo NOT IN ('particulares', 'institucionais', 'companhias de seguros'))
		RETURN -3
	BEGIN TRANSACTION
	IF (EXISTS (SELECT nifCli FROM Cliente WHERE nifCli = @nif)) BEGIN
		ROLLBACK
		RETURN -2
	END
	INSERT INTO Cliente (nifCli, nomeCli, moradaCli, telefoneCli, emailCli, tipoCli)
		VALUES (@nif, @nome, @morada, @telefone, @email, @tipo)
	COMMIT
END
GO

CREATE PROCEDURE AlteraCliente(@nif INT, @nome NOME_CLIENTE, @morada MORADA_CLIENTE, 
							   @telefone TELEFONE_CLIENTE, @email EMAIL_CLIENTE, @tipo TIPO_CLIENTE)
AS BEGIN
	IF (@nome = '' OR @tipo = '')
		RETURN -1
	SET @tipo = LOWER(@tipo)
	IF (@tipo NOT IN ('particulares', 'institucionais', 'companhias de seguros'))
		RETURN -3
	UPDATE Cliente 
		SET nomeCli=@nome, moradaCli=@morada, telefoneCli=@telefone, emailCli=@email, tipoCli=@tipo
		WHERE nifCli=@nif
	IF (@@ROWCOUNT=0)
		RETURN -2
END
GO

CREATE PROCEDURE RemoveCliente(@nif INT)
AS BEGIN
	DELETE FROM Cliente WHERE nifCli=@nif
	IF (@@ROWCOUNT=0)
		RETURN -2
END
GO

CREATE PROCEDURE InserirPeca(@ref REF_PECA, @designacao DESIGN_PECA, @preco MONEY) 
AS BEGIN
	IF (@ref = '' OR @ref IS NULL)
		RETURN -1
	BEGIN TRANSACTION
	IF (EXISTS (SELECT refP FROM Peca WHERE refP = @ref)) BEGIN
		ROLLBACK
		RETURN -2
	END
	INSERT INTO Peca (refP, designacaoP, precoP)
		VALUES (@ref, @designacao, @preco)
	COMMIT
END
GO

CREATE PROCEDURE AlterarPrecoPeca(@ref REF_PECA, @preco MONEY) 
AS BEGIN
	IF (@ref = '' OR @ref IS NULL)
		RETURN -1
	BEGIN TRANSACTION
	IF (NOT EXISTS (SELECT refP FROM Peca WHERE refP = @ref)) BEGIN
		ROLLBACK
		RETURN -2
	END
	UPDATE Peca SET precoP = @preco WHERE refP = @ref
	COMMIT
END
GO

CREATE PROCEDURE RegistarObra(@veiculo MATRICULA, @oficina INT, @actosDoc XML (ActosObraSchemaCollection) ) 
AS BEGIN
	IF (@veiculo = '' OR @veiculo IS NULL OR @oficina IS NULL)
		RETURN -1
	DECLARE @actos TABLE (id INT, departamento INT, horas REAL)
	DECLARE @idObra INT
	BEGIN TRANSACTION
		INSERT INTO @actos (id, departamento, horas)
			SELECT ref.value('id[1]', 'INT'), ref.value('departamento[1]', 'INT'), a.horasEstimadas
			FROM @actosDoc.nodes('/actos/acto') as actos(ref)
			LEFT JOIN Acto a ON a.idA=ref.value('id[1]', 'INT') AND 
								a.departamento=ref.value('departamento[1]', 'INT') AND
								a.oficina=@oficina
		IF (@@ROWCOUNT = 0 OR (EXISTS (SELECT horas FROM @actos WHERE horas IS NULL))) BEGIN
			ROLLBACK
			RETURN -2 /* pelo menos uma referência de acto, departamento ou oficina é inválida */
		END
		INSERT INTO Obra (oficina, dataRegistoO, estadoO, valorEstimado, totalHorasEstimado, veiculo)
			VALUES (@oficina, GETDATE(), 'espera peças', 0, 0, @veiculo)
		IF (@@ROWCOUNT = 0) BEGIN
			ROLLBACK
			RETURN -3
		END
		SET @idObra = SCOPE_IDENTITY()
		
		DECLARE @horasEstimadas REAL
		DECLARE @curActoId INT, @curDepId INT, @curHoras REAL, @curFunc INT
		DECLARE actosCur CURSOR FOR SELECT id, departamento FROM @actos
		SET @horasEstimadas = 0
		OPEN actosCur
		FETCH NEXT FROM actosCur INTO @curActoId, @curDepId, @curHoras
		WHILE (@@FETCH_STATUS = 0) BEGIN
			SET @horasEstimadas = @horasEstimadas + @curHoras
			/* 
			   TODO: encontrar funcionário habilitado para este acto, activo, 
					 e que não esteja atribuído a mais do que 10 obras em curso (i.e. marcadas; em
					 realização ou à espera de peças) 
			*/
			SET @curFunc = (
					SELECT TOP(1)
						...
				)
			IF (@curFunc IS NULL) BEGIN
				CLOSE actosCur
				DEALLOCATE actosCur
				ROLLBACK
				RETURN -4
			END
			FETCH NEXT FROM actosCur INTO @curActoId, @curDepId, @curHoras
		END
		CLOSE actosCur
		DEALLOCATE actosCur
	COMMIT
END
GO
