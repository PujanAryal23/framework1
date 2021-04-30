<#ftl/>
<#-- @ftlvariable name="self" type="java.util.Map<java.lang.String, com.lazerycode.jmeter.analyzer.parser.AggregatedResponses>" -->
<#-- @ftlvariable name="bytes" type="com.lazerycode.jmeter.analyzer.statistics.Samples" -->
<#-- @ftlvariable name="requests" type="com.lazerycode.jmeter.analyzer.statistics.Samples" -->
<!DOCTYPE html>
<html>
<head>
  <title>JMeter Test Results</title>
  <meta charset="utf-8">
  <style type="text/css">
    body {
      font: normal verdana, arial, helvetica;
      color: #000000;
    }

    table {
      border-collapse: collapse;
    }

    table tr td, table tr th {
    }

    table tr th {
      font-weight: bold;
      text-align: left;
      background: #a6caf0;
      white-space: nowrap;
    }

    table tr td {
      background: #eeeee0;
      white-space: nowrap;
    }

    h1 {
      margin: 0 0 5px;
      font: 165% verdana, arial, helvetica
    }

    h2 {
      margin-top: 1em;
      margin-bottom: 0.5em;
      font: bold 125% verdana, arial, helvetica
    }

    h3 {
      margin-bottom: 0.5em;
      font: bold 115% verdana, arial, helvetica
    }

    img {
      border-width: 0;
    }

    div {
      margin-bottom: 20px;
    }

    div.details ul li {
      list-style: none;
    }
  </style>
</head>
<body>
  <h1>JMeter Summary</h1>
<#if !self?keys?has_content>
  <p>Results file is empty.</p>
<#else>
  <div class="aggregations">
    <#list self?keys as key>
      <#assign aggregatedResponses=self(key)/>
      <#assign bytes=aggregatedResponses.size/>
      <#assign requests=aggregatedResponses.duration/>
      <#include "aggregatedResponse.ftl" />
    </#list>
  </div>
</#if>
</body>
</html>