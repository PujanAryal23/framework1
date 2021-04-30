/*Run this script to create test data for performance testing. It can be run using SQLPlus.*/
SET SQLBLANKLINES ON
SET ECHO OFF
SET TERMOUT OFF
SET TRIMSPOOL ON
SET SERVEROUTPUT ON
SET PAGESIZE 0
SET LINESIZE 2000
SET FEEDBACK OFF
SPOOL C:\Project\Nucleus\TestData.csv

PROMPT CLASEQ,CLASUB;
DECLARE
sql_stmt VARCHAR2(2000);
no_of_users NUMBER(4) := 100;
client VARCHAR2(100) := 'LOADT'; 
TYPE tmp_tbm IS TABLE OF VARCHAR2(200);
prov_tbl tmp_tbm;
 BEGIN 
 sql_stmt := 'WITH providerdataset1 AS
(
SELECT DISTINCT prvseq as providersequence, provider_id as providerid, Row_Number() over
(ORDER BY 1) rn
FROM ' || client ||'.provider
WHERE ROWNUM <= 10
),
providerdataset2  AS
(
SELECT providername, Row_Number() over (ORDER BY 1)  rn FROM (

SELECT DISTINCT full_or_facility_name as providername
FROM ' || client ||'.provider
)
WHERE ROWNUM <=10

),
appealdataset1 AS
(
SELECT DISTINCT appealseq as appealsequence, Row_Number() over (ORDER BY 1)  rn
FROM ats.appeal a
JOIN ref_appeal_status b
ON a.status = b.status_code
WHERE Upper(status) NOT  IN (''O'',''C'',''T'',''D'')
AND active = ''T''
AND clientcode = '''|| client ||'''
AND ROWNUM <= ' || no_of_users ||'
),
ClaimAction AS
(
SELECT DISTINCT claseq, clasub, Row_Number() over (ORDER BY 1)  rn
FROM '|| client ||'.claim
WHERE status= ''U''
AND has_appeal=''F''
AND ROWNUM <= ' || no_of_users ||'
)

SELECT claseq  || '','' || clasub FROM
ClaimAction';
EXECUTE IMMEDIATE sql_stmt
 BULK COLLECT INTO prov_tbl;
 FOR i IN 1..prov_tbl.count LOOP
 Dbms_Output.put_line(prov_tbl(i));
 END LOOP;
 END;
/
SPOOL OFF