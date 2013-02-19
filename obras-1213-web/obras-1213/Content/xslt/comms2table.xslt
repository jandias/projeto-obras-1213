<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns:myns="si2.isel.pt/2013/TrabFinal">

  <xsl:output method="html" omit-xml-declaration="yes"/>

  <xsl:template match="/">

    <xsl:for-each select="//myns:departamento">
      <h3>
        Departamento ref.<span>
          <xsl:value-of select="../@codOfic"/>-<xsl:value-of select="@codDep"/>
        </span>
      </h3>

      <xsl:for-each select=".//myns:comunicado">
        <table border="0">
          <tr>
            <th></th>
            <td>
              <i>
                [ref.<xsl:value-of select="@idCom"/>] -
                <xsl:value-of select="myns:tipo"/>
              </i>
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
          <xsl:if test="myns:urlPrint">
            <tr>
              <th>Link para impressão:</th>
              <td>
                <a href="{myns:urlPrint}">
                  <xsl:value-of select="myns:urlPrint"/>
                </a>
              </td>
            </tr>
          </xsl:if>
          <xsl:if test="myns:urlMedia">
            <tr>
              <th>Link Multimédia:</th>
              <td>
                <a href="{myns:urlMedia}">
                  <xsl:value-of select="myns:urlMedia"/>
                </a>
              </td>
            </tr>
          </xsl:if>
          <xsl:if test="myns:conteudo">
            <tr>
              <th></th>
              <td>
                <xsl:value-of select="myns:conteudo"/>
              </td>
            </tr>
          </xsl:if>
        </table>
        <br/>
      </xsl:for-each>

    </xsl:for-each>


  </xsl:template>

</xsl:stylesheet>