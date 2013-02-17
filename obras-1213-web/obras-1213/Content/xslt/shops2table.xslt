<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" omit-xml-declaration="yes" />

    <xsl:template match="/">
      <table class="table">
        <tr>
          <th> </th>
          <th>Nome</th>
          <th>Morada</th>
          <th>Telefone</th>
          <th>Fax</th>
          <th>NIF</th>
          <th>Responsável</th>
        </tr>
        <xsl:for-each select="Oficinas/Oficina">
          <tr>
            <td>
              <xsl:value-of select="@codOfic"/>
            </td>
            <td>
              <xsl:value-of select="@nomeOfic"/>
            </td>
            <td>
              <xsl:value-of select="@moradaOfic"/>
            </td>
            <td>
              <xsl:value-of select="@telefoneOfic"/>
            </td>
            <td>
              <xsl:value-of select="@faxOfic"/>
            </td>
            <td>
              <xsl:value-of select="@nifOfic"/>
            </td>
            <td>
              <xsl:value-of select="Responsavel/@nomeFunc"/>
            </td>
          </tr>
        </xsl:for-each>
      </table>
    </xsl:template>

  </xsl:stylesheet>
