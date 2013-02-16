USE Obras

DROP TABLE FuncionarioDepartamento
DROP TABLE ProfissaoDepartamento
DROP TABLE LinhaFactura
DROP TABLE Factura
DROP TABLE Cliente
DROP TABLE Reserva
DROP TABLE Peca
DROP TABLE ObraContem
DROP TABLE Obra
DROP TABLE Veiculo
DROP TABLE Habilitado
DROP TABLE Acto
DROP TABLE Comunicado
DROP TABLE Departamento
DROP TABLE Oficina
DROP TABLE Funcionario
DROP TABLE Profissao
DROP TABLE HistoricoAnualObras
DROP TABLE HistoricoPecas

DROP TYPE NOME_CLIENTE
DROP TYPE MORADA_CLIENTE
DROP TYPE TELEFONE_CLIENTE
DROP TYPE EMAIL_CLIENTE
DROP TYPE TIPO_CLIENTE
DROP TYPE REF_PECA
DROP TYPE DESIGN_PECA
DROP TYPE MATRICULA

CREATE TYPE MATRICULA FROM VARCHAR(10)

CREATE TYPE	REF_PECA	FROM VARCHAR(15)
CREATE TYPE DESIGN_PECA	FROM VARCHAR(30)

CREATE TYPE NOME_CLIENTE FROM VARCHAR(300)
CREATE TYPE MORADA_CLIENTE FROM VARCHAR(300)
CREATE TYPE TELEFONE_CLIENTE FROM VARCHAR(15)
CREATE TYPE EMAIL_CLIENTE FROM VARCHAR(35)
CREATE TYPE TIPO_CLIENTE FROM VARCHAR(21)

CREATE TABLE Profissao(
	idProf INT IDENTITY(1,1) PRIMARY KEY,
	designacaoProf VARCHAR(30) NOT NULL
)

CREATE TABLE Funcionario(
	codFunc INT IDENTITY(1,1) PRIMARY KEY,
	nifFunc INT UNIQUE NOT NULL,
	nomeFunc VARCHAR(300) NOT NULL,
	telefoneFunc INT,
	emailFunc VARCHAR(35),
	estadoFunc VARCHAR(12) NOT NULL CHECK (estadoFunc IN ('activo', 'baixa médica', 'férias', 'falecido', 'demitido')),
	profissao INT NOT NULL REFERENCES Profissao (idProf)
)

CREATE TABLE Oficina(
	codOfic INT IDENTITY(1,1) PRIMARY KEY,
	nifOfic INT UNIQUE NOT NULL,
	nomeOfic VARCHAR(50) NOT NULL,
	moradaOfic VARCHAR(300) NOT NULL,
	telefoneOfic INT NOT NULL,
	faxOfic INT,
	responsavel INT REFERENCES Funcionario (codFunc) ON DELETE SET NULL
)

CREATE TABLE Departamento(
	codDep INT IDENTITY(1,1),
	oficina INT NOT NULL REFERENCES Oficina (codOfic) ON DELETE CASCADE,
	nomeDep VARCHAR(30),
	responsavel INT NOT NULL REFERENCES Funcionario (codFunc),
	PRIMARY KEY (codDep, oficina)
)

CREATE TABLE ProfissaoDepartamento(
	departamento INT NOT NULL,
	oficina INT NOT NULL,
	profissao INT NOT NULL REFERENCES Profissao (idProf) ON DELETE CASCADE,
	PRIMARY KEY (departamento, oficina, profissao),
	FOREIGN KEY (departamento, oficina) REFERENCES Departamento (codDep, oficina) ON DELETE CASCADE
)

CREATE TABLE FuncionarioDepartamento(
	departamento INT NOT NULL,
	oficina INT NOT NULL,
	funcionario INT NOT NULL REFERENCES Funcionario (codFunc) ON DELETE CASCADE,
	PRIMARY KEY (departamento, oficina, funcionario),
	FOREIGN KEY (departamento, oficina) REFERENCES Departamento (codDep, oficina) ON DELETE CASCADE
)

CREATE TABLE Comunicado(
	idCom INT IDENTITY(1,1) PRIMARY KEY,
	conteudoCom XML,
	departamento INT NOT NULL,
	oficina INT NOT NULL,
	FOREIGN KEY (departamento,oficina) REFERENCES Departamento (codDep,oficina) ON DELETE CASCADE
)


CREATE TABLE Acto(
	idA INT IDENTITY(1,1),
	departamento INT NOT NULL,
	oficina INT NOT NULL,
	designacaoA VARCHAR(50) NOT NULL,
	horasEstimadas REAL NOT NULL,
	PRIMARY KEY (idA,departamento,oficina),
	FOREIGN KEY (departamento,oficina) REFERENCES Departamento (codDep,oficina) ON DELETE CASCADE
)

CREATE TABLE Habilitado(
	acto INT NOT NULL,
	departamento INT NOT NULL,
	oficina INT NOT NULL,
	funcionario INT NOT NULL REFERENCES Funcionario (codFunc),
	PRIMARY KEY (acto, departamento, oficina, funcionario),
	FOREIGN KEY (acto,departamento,oficina) REFERENCES Acto (idA,departamento,oficina) ON DELETE CASCADE
)

CREATE TABLE Veiculo(
	matricula MATRICULA PRIMARY KEY,
	marca VARCHAR(12) NOT NULL,
	modelo VARCHAR(20) NOT NULL,
	nomeProprietario VARCHAR(100)
)

CREATE TABLE Obra(
	codO INT IDENTITY(1,1),
	oficina INT REFERENCES Oficina (codOfic),
	dataRegistoO DATE NOT NULL,
	estadoO VARCHAR(13) NOT NULL CHECK (estadoO IN ('marcada', 'em realização', 'espera peças', 'concluída', 'facturada', 'paga')),
	valorEstimado MONEY NOT NULL,
	totalHorasEstimado REAL NOT NULL,
	veiculo MATRICULA NOT NULL REFERENCES Veiculo (matricula),
	PRIMARY KEY(codO,oficina)
)
	
CREATE TABLE ObraContem(
	obra INT NOT NULL,
	oficina INT NOT NULL,
	acto INT NOT NULL,
	departamento INT NOT NULL,
	funcionario INT REFERENCES Funcionario (codFunc),
	horasRealizadas REAL NOT NULL,
	estaConcluido BIT NOT NULL
	PRIMARY KEY (obra,oficina,acto,departamento,funcionario),
	FOREIGN KEY (acto,departamento,oficina,funcionario) REFERENCES Habilitado (acto,departamento,oficina,funcionario) ON DELETE CASCADE
)

CREATE TABLE Peca(
	refP REF_PECA PRIMARY KEY,
	designacaoP DESIGN_PECA NOT NULL,
	precoP MONEY NOT NULL
)

CREATE TABLE HistoricoPecas(
	refP REF_PECA NOT NULL,
	designacaoP DESIGN_PECA NOT NULL,
	precoP MONEY NOT NULL,
	dataP DATETIME NOT NULL,
	PRIMARY KEY (refP, precoP, dataP)
)

CREATE TABLE Reserva(
	peca VARCHAR(15) REFERENCES Peca(refP) ON UPDATE CASCADE,
	oficina INT NOT NULL,
	obra INT NOT NULL, 
	quantP INT NOT NULL CHECK (quantP > 0),
	PRIMARY KEY (peca, oficina, obra),
	FOREIGN KEY (obra, oficina) REFERENCES Obra (codO, oficina) ON DELETE CASCADE
)

CREATE TABLE Cliente(
	numCli INT IDENTITY(1,1) PRIMARY KEY,
	nifCli INT UNIQUE NOT NULL,
	nomeCli NOME_CLIENTE NOT NULL,
	moradaCli MORADA_CLIENTE,
	telefoneCli TELEFONE_CLIENTE,
	emailCli EMAIL_CLIENTE,
	tipoCli TIPO_CLIENTE NOT NULL CHECK (tipoCli IN ('particulares', 'institucionais', 'companhias de seguros'))
)

CREATE TABLE Factura(
	numFact INT IDENTITY(1,1) PRIMARY KEY,
	dataFact DATE NOT NULL,
	estadoFact VARCHAR(7) NOT NULL CHECK (estadoFact IN ('emitida', 'paga')),
	desconto INT NOT NULL,
	totalFactura MONEY,
	obra INT NOT NULL,
	oficina INT NOT NULL,
	cliente INT NOT NULL REFERENCES Cliente (numCli),
	FOREIGN KEY (obra,oficina) REFERENCES Obra (codO,oficina)
)

CREATE TABLE LinhaFactura(
	nLinha INT,
	factura INT NOT NULL REFERENCES Factura (numFact),
	descricaoLinha VARCHAR(80) NOT NULL,
	precoUnit MONEY NOT NULL,
	quant REAL NOT NULL,
	totalLinha MONEY NOT NULL,
	PRIMARY KEY(nLinha, factura)
)

CREATE TABLE HistoricoAnualObras (
	anoHist INT PRIMARY KEY,
	conteudo XML
)
