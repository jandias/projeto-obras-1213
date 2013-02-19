<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns:myns="si2.isel.pt/2013/TrabFinal">
  
    <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <html>
      <head>
        <style type="text/css">
          th { white-space: nowrap; text-align: right; }
          h3 { text-align: center; }
        </style>
      </head>
      <body>
        <h1>Comunicados da empresa</h1>

        <xsl:for-each select="//myns:departamento">
          <h3>Departamento ref.<span>
            <xsl:value-of select="../@codOfic"/>-<xsl:value-of select="@codDep"/></span>
          </h3>
        
          <xsl:for-each select=".//myns:comunicado">
            <table border="0">
              <tr>
                <th></th>
                <td><i>
                  [ref.<xsl:value-of select="@idCom"/>] -
                  <xsl:value-of select="myns:tipo"/></i>
                </td>
              </tr>
              <tr>
                <th>Autor:</th>
                <td>
                  <xsl:value-of select="myns:autor"/>
                </td>
              </tr>
              <tr>
                <th>Data de criação:</th>
                <td>
                  <xsl:value-of select="myns:data"/>
                </td>
              </tr>
              <xsl:if test="myns:data[@entrada-em-vigor]">
                <tr>
                  <th>Data de efeito:</th>
                  <td>
                    <xsl:value-of select="myns:data/@entrada-em-vigor"/>
                  </td>
                </tr>
              </xsl:if>
              <tr>
                <th></th>
                <td>
                  <xsl:value-of select="myns:conteudo"/>
                </td>
              </tr>
            </table>
            <br/>
          </xsl:for-each>

        </xsl:for-each>
      </body>
    </html>
    
  </xsl:template>
  
  <!--
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>
  -->
</xsl:stylesheet>
