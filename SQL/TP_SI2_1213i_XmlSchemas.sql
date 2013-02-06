USE TP_SI2_1213i

DROP XML SCHEMA COLLECTION ActosObraSchemaCollection

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
