USE TP_SI2_1213i

DROP VIEW ListaPecas
DROP VIEW ObrasNaoTerminadas

DROP FUNCTION EscolheFuncionario

DROP PROCEDURE RemoveCliente
DROP PROCEDURE AlteraCliente
DROP PROCEDURE InsereCliente
DROP PROCEDURE InserirPeca
DROP PROCEDURE AlterarPrecoPeca
DROP PROCEDURE RegistarObra
DROP PROCEDURE TerminarObra
DROP PROCEDURE AtribuiActoFuncionario
DROP PROCEDURE PagarFactura
DROP PROCEDURE FacturarObra
DROP PROCEDURE RelatorioAnual

DROP TRIGGER RegistaHistoricoPeca 
DROP TRIGGER ValidaProfissaoFuncionarioDepartamento 
DROP TRIGGER ValidaFuncionarioObra 
DROP TRIGGER ValidaClienteObra 
DROP TRIGGER ImpedeRemocaoFuncionario 
DROP TRIGGER ImpedeRemocaoObra 
GO

/* b. Realizar a consulta da lista de Peças e respectivos preços, apresentando, para cada peça, o número de obras em
que foi requisitada. */
CREATE VIEW ListaPecas
AS
	SELECT p.refP, p.designacaoP, p.precoP, COUNT(r.obra) as nobras FROM Peca p
		LEFT JOIN Reserva r ON p.refP=r.peca
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

/* a. Inserir, remover e actualizar informação sobre um Cliente. */

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

/* d. Inserir uma nova peça e alterar o preço de uma peça. */

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

/*
e. Registar uma Obra com o respectivo conjunto de actos. A selecção do funcionário para cada acto deverá ser
automática: observando quais os funcionários disponíveis para realização do respectivo acto, escolher o que tiver
menos obras em curso, indicando informação sobre os respectivos actos. Nota: de entre os funcionários disponíveis
para um acto, apenas pode ser seleccionado um funcionário no estado activo.
*/

CREATE FUNCTION EscolheFuncionario(@acto INT, @departamento INT, @oficina INT)
RETURNS INT
AS BEGIN
	DECLARE @func INT

	/* 
		Encontrar funcionário habilitado para um acto, activo, 
		e que não esteja atribuído a mais do que 10 obras em curso (i.e. marcadas; em
		realização ou à espera de peças) 
	*/
	SELECT TOP(1) @func=h.funcionario FROM Habilitado h
		JOIN Funcionario f ON h.funcionario=f.codFunc AND f.estadoFunc='activo'
		LEFT JOIN ObraContem oc ON oc.acto=h.acto AND oc.departamento=h.departamento AND oc.funcionario=h.funcionario AND oc.oficina=h.oficina
		LEFT JOIN Obra o ON o.codO=oc.obra AND o.oficina = oc.oficina
		WHERE h.acto=@acto AND h.departamento=@departamento AND h.oficina=@oficina 
		GROUP BY h.funcionario
		HAVING SUM(CASE WHEN o.estadoO IN ('marcada', 'em realização', 'espera peças') THEN 1 ELSE 0 END) < 10
		ORDER BY SUM(CASE WHEN o.estadoO IN ('marcada', 'em realização', 'espera peças') THEN 1 ELSE 0 END) ASC

	RETURN @func
END
GO

CREATE PROCEDURE RegistarObra(@veiculo MATRICULA, @oficina INT, @actosDoc XML (ActosObraSchemaCollection) ) 
AS BEGIN
	IF (@veiculo = '' OR @veiculo IS NULL OR @oficina IS NULL)
		RETURN -1
		
	DECLARE @actos TABLE (id INT, departamento INT, horas REAL)
	DECLARE @idObra INT
	DECLARE @horasEstimadas REAL
	DECLARE @curActoId INT, @curDepId INT, @curHoras REAL, @curFunc INT
	
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
		
		SET @horasEstimadas = 0
		DECLARE actosCur CURSOR FOR SELECT id, departamento FROM @actos
		OPEN actosCur
		FETCH NEXT FROM actosCur INTO @curActoId, @curDepId, @curHoras
		WHILE (@@FETCH_STATUS = 0) BEGIN
			SET @horasEstimadas = @horasEstimadas + @curHoras
			SET @curFunc = (SELECT dbo.EscolheFuncionario(@curActoId, @curDepId, @oficina))
			IF (@curFunc IS NULL) BEGIN
				CLOSE actosCur
				DEALLOCATE actosCur
				ROLLBACK
				RETURN -4 /* não existem funcionários disponíveis para todos os actos */
			END
			INSERT INTO ObraContem (obra, oficina, acto, departamento, funcionario, horasRealizadas, estaConcluido)
				VALUES (@idObra, @oficina, @curActoId, @curDepId, @curFunc, 0, 0) 
			FETCH NEXT FROM actosCur INTO @curActoId, @curDepId, @curHoras
		END
		CLOSE actosCur
		DEALLOCATE actosCur
		
		UPDATE Obra SET totalHorasEstimado=@horasEstimadas WHERE codO=@idObra
	COMMIT
END
GO

/* f. Terminar uma obra. */
CREATE PROCEDURE TerminarObra(@obra INT, @oficina INT)
AS BEGIN
	BEGIN TRANSACTION
		IF (NOT EXISTS (SELECT codO FROM Obra WHERE codO=@obra AND oficina=@oficina)) BEGIN
			ROLLBACK
			RETURN -2 /* referência de obra ou de oficina inválida */
		END
		UPDATE Obra SET estadoO = 'concluída'
	COMMIT
END
GO

/* g. Listar todas as obras ainda não terminadas, apresentado o seu estado actual e informação dos respectivos actos. */
CREATE VIEW ObrasNaoTerminadas
AS 
	SELECT o.codO, o.estadoO, a.designacaoA, oc.estaConcluido, oc.horasRealizadas, f.nomeFunc FROM Obra o
		JOIN ObraContem oc ON oc.obra=o.codO AND oc.oficina=o.oficina
		JOIN Acto a ON a.idA=oc.acto
		JOIN Funcionario f ON f.codFunc=oc.funcionario
		WHERE o.estadoO IN ('marcada', 'em realização', 'espera peças')
GO

/* h. Produzir informação anual com informação sobre todas as obras concluídas e respectivos montantes realizados. */
CREATE PROCEDURE RelatorioAnual(@ano INT, @relatorio XML OUTPUT)
AS BEGIN
	BEGIN TRANSACTION
		DECLARE @report XML
		SET @report = (
			SELECT o.codO, o.dataRegistoO, o.valorEstimado, o.totalHorasEstimado, 
				   SUM(oc.horasRealizadas) as horasRealizadas,
				   SUM(CASE WHEN f.totalFactura IS NULL THEN 0 ELSE f.totalFactura END) as valorRealizado
			FROM Obra o
			JOIN ObraContem oc ON oc.obra=o.codO AND oc.oficina=o.oficina
			LEFT JOIN Factura f ON f.obra=o.codO AND f.oficina=o.oficina
			WHERE o.estadoO NOT IN ('marcada', 'em realização', 'espera peças')
			GROUP BY o.codO, o.dataRegistoO, o.valorEstimado, o.totalHorasEstimado
			FOR XML RAW, TYPE, ROOT('obrasConcluidas')
		)
		IF (EXISTS (SELECT anoHist FROM HistoricoAnualObras WHERE anoHist=@ano))
			UPDATE HistoricoAnualObras SET conteudo=@report WHERE anoHist=@ano
		ELSE
			INSERT INTO HistoricoAnualObras (anoHist, conteudo) VALUES (@ano, @report)
	COMMIT
END
GO

/* i. Facturar uma Obra e fazer o respectivo pagamento. */

CREATE PROCEDURE FacturarObra(@obra INT, @oficina INT, @cliente INT)
AS BEGIN
	BEGIN TRANSACTION
		IF ((NOT EXISTS (SELECT codO FROM Obra WHERE codO=@obra AND oficina=@oficina)) OR 
			(NOT EXISTS (SELECT numCli FROM Cliente WHERE numCli=@cliente))) BEGIN
			ROLLBACK
			RETURN -2 /* referência de obra ou de cliente inválida */
		END
		IF (EXISTS (SELECT codO FROM Obra WHERE codO=@obra AND oficina=@oficina AND estadoO IN ('facturada', 'paga'))) BEGIN
			ROLLBACK
			RETURN -3 /* obra já facturada */
		END
		
		DECLARE @desconto REAL, @totalHoras REAL, @valorTotal MONEY, @numLinha INT
		SET @desconto = 0
		SET @totalHoras = 0
		SET @valorTotal = 0
		SET @numLinha = 1
		
		DECLARE @idFactura INT
		INSERT INTO Factura (dataFact, estadoFact, desconto, totalFactura, obra, oficina, cliente)
			VALUES (GETDATE(), 'emitida', 0, 0, @obra, @oficina, @cliente)
		SET @idFactura = SCOPE_IDENTITY()
		
		/* inserir mão de obra */
		DECLARE @designacao VARCHAR(MAX), @horas REAL
		DECLARE actosCur CURSOR FOR 
			SELECT a.designacaoA, oc.horasRealizadas FROM ObraContem oc
				JOIN Acto a ON a.idA=oc.acto AND a.departamento=oc.departamento AND a.oficina=oc.oficina
				WHERE oc.obra=@obra AND oc.oficina=@oficina
		OPEN actosCur
		FETCH NEXT FROM actosCur INTO @designacao, @horas
		WHILE (@@FETCH_STATUS=0) BEGIN
			SET @totalHoras = @totalHoras + @horas
			SET @valorTotal = @valorTotal + (30 * @horas)
			INSERT INTO LinhaFactura (nLinha, factura, descricaoLinha, precoUnit, quant, totalLinha)
				VALUES (@numLinha, @idFactura, @designacao, 30, @horas, 30 * @horas)
			SET @numLinha = @numLinha + 1 
			FETCH NEXT FROM actosCur INTO @designacao, @horas
		END
		CLOSE actosCur
		DEALLOCATE actosCur
		
		/* inserir peças */
		DECLARE @quantidade INT, @preco MONEY
		DECLARE pecasCur CURSOR FOR
			SELECT r.quantP, p.designacaoP, p.precoP FROM Reserva r
				JOIN Peca p ON r.peca=p.refP
				WHERE r.obra=@obra AND r.oficina=@oficina
		OPEN pecasCur
		FETCH NEXT FROM pecasCur INTO @quantidade, @designacao, @preco
		WHILE (@@FETCH_STATUS=0) BEGIN
			SET @valorTotal = @valorTotal + (@quantidade * @preco)
			INSERT INTO LinhaFactura (nLinha, factura, descricaoLinha, precoUnit, quant, totalLinha)
				VALUES (@numLinha, @idFactura, @designacao, @preco, @quantidade, @quantidade * @preco)
			SET @numLinha = @numLinha + 1 
			FETCH NEXT FROM pecasCur INTO @quantidade, @designacao, @preco
		END
		
		/* calcular desconto */
		DECLARE @numObras INT
		SELECT @numObras=COUNT(*) FROM Obra o 
			JOIN Factura f ON f.obra=o.codO AND f.oficina = o.oficina AND f.cliente=@cliente
			WHERE o.dataRegistoO >= DATEADD(month, -1, GETDATE()) AND
				  o.codO=@obra AND o.oficina=@oficina
		IF (@numObras>10)
			SET @desconto=0.1
		ELSE IF (@numObras>5)
			SET @desconto=0.05
		
		UPDATE Factura SET desconto=@desconto*100, totalFactura=@valorTotal-(@valorTotal*@desconto)
			WHERE numFact=@idFactura
	COMMIT
END
GO

CREATE PROCEDURE PagarFactura(@factura INT)
AS BEGIN
	BEGIN TRANSACTION
		DECLARE @obra INT, @oficina INT, @estado VARCHAR(MAX)
		SELECT @obra=obra, @oficina=oficina, @estado=estadoFact FROM Factura WHERE numFact=@factura
		IF (@obra IS NULL OR @oficina IS NULL) BEGIN
			ROLLBACK
			RETURN -2 /* referência de factura inválida */
		END
		IF (@estado='paga') BEGIN
			ROLLBACK
			RETURN -3 /* factura já paga */
		END

		UPDATE Factura SET estadoFact='paga' WHERE numFact=@factura
		UPDATE Obra SET estadoO='paga' WHERE codO=@obra AND oficina=@oficina
	COMMIT
END
GO

/* c. Afectar um funcionário a um Departamento, no contexto de um Acto. */
CREATE PROCEDURE AtribuiActoFuncionario(@acto INT, @departamento INT, @oficina INT, @funcionario INT)
AS BEGIN
	BEGIN TRANSACTION
		IF (NOT EXISTS (SELECT funcionario FROM Habilitado 
			WHERE funcionario=@funcionario AND acto=@acto AND departamento=@departamento AND oficina=@oficina)) BEGIN
			INSERT INTO Habilitado (funcionario, acto, departamento, oficina)
				VALUES (@funcionario, @acto, @departamento, @oficina)
			IF (@@ROWCOUNT=0) BEGIN
				ROLLBACK
				RETURN -2 /* pelo menos uma referência é inválida */
			END
		END
		IF (NOT EXISTS (SELECT funcionario FROM FuncionarioDepartamento WHERE funcionario=@funcionario)) BEGIN
			INSERT INTO FuncionarioDepartamento (departamento, oficina, funcionario)
				VALUES (@departamento, @oficina, @funcionario) 
			IF (@@ROWCOUNT=0) BEGIN
				ROLLBACK
				RETURN -3 /* ou pelo menos uma referência é inválida ou o funcionário não pode estar neste departamento */
			END
		END
	COMMIT
END
GO
