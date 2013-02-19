USE Obras

DROP XML SCHEMA COLLECTION ActosObraSchemaCollection
DROP XML SCHEMA COLLECTION ComunicadoSchemaCollection

CREATE XML SCHEMA COLLECTION ActosObraSchemaCollection AS 
N'<?xml version="1.0"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:element name="actos">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="acto" minOccurs="1" maxOccurs="unbounded">
          <xs:complexType>
            <xs:all>
              <xs:element name="id" type="xs:int" minOccurs="1" />
              <xs:element name="departamento" type="xs:int" minOccurs="1" />
            </xs:all>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>  
</xs:schema>
'

CREATE XML SCHEMA COLLECTION ComunicadoSchemaCollection AS 
N'<?xml version="1.0"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:myns="si2.isel.pt/2013/TrabFinal" targetNamespace="si2.isel.pt/2013/TrabFinal"
 elementFormDefault="qualified" attributeFormDefault="unqualified">
  <xs:element name="comunicado">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="tipo" type="myns:tipoType" minOccurs="1"/>
        <xs:element name="autor" type="xs:string" minOccurs="1"/>
        <xs:element name="data" type="myns:dataType" minOccurs="1"/>
        <xs:choice>
          <xs:sequence>
            <xs:element name="conteudo" type="xs:string" minOccurs="1" maxOccurs="1"/>
            <xs:element name="urlPrint" type="xs:anyURI" minOccurs="0" maxOccurs="1"/>
            <xs:element name="urlMedia" type="xs:anyURI" minOccurs="0" maxOccurs="1"/>
          </xs:sequence>
          <xs:sequence>
            <xs:element name="urlPrint" type="xs:anyURI" minOccurs="1" maxOccurs="1"/>
            <xs:element name="urlMedia" type="xs:anyURI" minOccurs="0" maxOccurs="1"/>
          </xs:sequence>
          <xs:sequence>
            <xs:element name="urlMedia" type="xs:anyURI" minOccurs="1" maxOccurs="1"/>
          </xs:sequence>
        </xs:choice>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  <xs:complexType name="dataType">
    <xs:simpleContent>
      <xs:extension base="xs:date">
        <xs:attribute name="entrada-em-vigor" type="xs:date" default="2014-01-10" />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:simpleType name="tipoType">
    <xs:restriction base="xs:string">
      <xs:enumeration value="aviso"/>
      <xs:enumeration value="informação"/>
      <xs:enumeration value="legislação"/>
      <xs:enumeration value="anúncio"/>
    </xs:restriction>
  </xs:simpleType>
</xs:schema>
'
