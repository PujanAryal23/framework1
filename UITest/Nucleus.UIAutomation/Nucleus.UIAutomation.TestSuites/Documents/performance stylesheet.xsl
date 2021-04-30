<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <head>
  <style>
  h2{
  color: #999;
  font-size:18px;
  padding: 7px;
  }
	th {
   padding: 7px;
	}
	td{
	padding: 5px;
	text-align: center;
	}
</style>
  </head>
  <body>
  <h2>Page Performance Measurement</h2>
  <table border="1">
    <tr bgcolor="#0CA910" style= "color:#fff">
      <th>Tested Time</th>
      <th>Page Name</th>
      <th>Fully Loaded Time</th>
      <th>Connection Time</th>
      <th>DOM Completed Time</th>
      <th>Fetch Time</th>
    </tr>
    <xsl:for-each select="Performance">
    <tr>
      <td><xsl:value-of select="TestTime"/></td>
      <td><xsl:value-of select="PageName"/></td>
      <td><xsl:value-of select="PageFullyLoadedTime"/></td>
      <td><xsl:value-of select="PageConnectTime"/></td>
      <td><xsl:value-of select="PageDomCompleteTime"/></td>
      <td><xsl:value-of select="PageFetchTime"/></td>
    </tr>
    </xsl:for-each>
  </table>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>