USE Obras

DELETE FROM Habilitado
DELETE FROM Acto
DELETE FROM FuncionarioDepartamento
DELETE FROM ProfissaoDepartamento
DELETE FROM Departamento
DELETE FROM Oficina
DELETE FROM Funcionario
DELETE FROM Profissao

SET IDENTITY_INSERT Profissao ON
INSERT INTO Profissao (idProf, designacaoProf)
VALUES 
	(1, 'Recepcionista'), (2, 'Lavador'), (3, 'Mecânico'), (4, 'Bate-chapas'), (5, 'Pintor'),
	(6, 'Fiel de armazém'), (7, 'Contabilista'), (8, 'Operador de registo de dados'),
	(9, 'Administrativo'), (10, 'Tesoureiro')
SET IDENTITY_INSERT Profissao OFF

SET IDENTITY_INSERT Funcionario ON
INSERT INTO Funcionario (codFunc, nifFunc, nomeFunc, telefoneFunc, emailFunc, estadoFunc, profissao)
VALUES 
	(1, 123456789, 'Silva', '214365879', 'silva@example.com', 'activo', 1),
	(2, 123456780, 'Martins', '314365879', 'martins@example.com', 'activo', 2),
	(3, 123456781, 'Santos', '414365879', 'santos@example.com', 'activo', 3),
	(4, 123456782, 'Sousa', '514365879', 'sousa@example.com', 'activo', 4),
	(5, 123456783, 'Manuel', '614365879', 'manuel@example.com', 'activo', 5),
	(6, 123456784, 'Maria', '714365879', 'maria@example.com', 'activo', 6),
	(7, 123456785, 'José', '814365879', 'jose@example.com', 'activo', 7),
	(8, 123456786, 'Ana', '914365879', 'ana@example.com', 'activo', 8),
	(9, 123456787, 'Cristina', '124365879', 'cristina@example.com', 'activo', 9),
	(10, 123456788, 'Patricia', '134365879', 'patricia@example.com', 'activo', 10)
SET IDENTITY_INSERT Funcionario OFF

SET IDENTITY_INSERT Oficina ON
INSERT INTO Oficina (codOfic, nifOfic, nomeOfic, telefoneOfic, faxOfic, responsavel, moradaOfic)
	VALUES (1, 765432345, 'Oficina principal', 945275230, 30256817, 10, 'Lisboa')
SET IDENTITY_INSERT Oficina OFF

SET IDENTITY_INSERT Departamento ON
INSERT INTO Departamento (codDep, oficina, nomeDep, responsavel)
VALUES
	(1, 1, 'Recepção', 10),
	(2, 1, 'Estação de serviço', 10),
	(3, 1, 'Electricidade', 10),
	(4, 1, 'Mecânica', 10),
	(5, 1, 'Colisões', 10),
	(6, 1, 'Almoxarifado', 10),
	(7, 1, 'Contabilidade', 10),
	(8, 1, 'Administrativo', 10),
	(9, 1, 'Tesouraria', 10)
SET IDENTITY_INSERT Departamento OFF

INSERT INTO ProfissaoDepartamento (departamento, oficina, profissao)
VALUES
	(1, 1, 1), (2, 1, 2), (3, 1, 2), (4, 1, 3), (5, 1, 4), (5, 1, 5), (6, 1, 6),
	(7, 1, 7), (7, 1, 8), (8, 1, 9), (9, 1, 10)
	
SET IDENTITY_INSERT Acto ON
INSERT INTO Acto (idA, departamento, oficina, designacaoA, horasEstimadas)
VALUES
	(1, 2, 1, 'Lavagem', 0.5),
	(2, 3, 1, 'Montagem auto-rádio', 0.5),
	(3, 3, 1, 'Afinação faróis', 0.25),
	(4, 4, 1, 'Revisão geral', 1),
	(5, 4, 1, 'Substituição óleo', 0.5),
	(6, 5, 1, 'Substituição pára-choques', 1.5)
SET IDENTITY_INSERT Acto OFF
