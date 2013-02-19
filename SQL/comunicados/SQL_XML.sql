USE Obras;

WITH XMLNAMESPACES ('si2.isel.pt/2013/TrabFinal' as myns)
select oficina as [oficina/@id] from Comunicado
	for xml path,root('myns:comunicados')

select oficina as [oficina/@id] from Comunicado for xml path

SELECT conteudoCom from Comunicado

SELECT conteudoCom.query('for $myns:autor in . return {$autor}') as result from Comunicado

WITH XMLNAMESPACES ('si2.isel.pt/2013/TrabFinal' as myns, 'http://www.w3.org/2001/XMLSchema-instance' as xsi)
select codOfic as [@codOfic], 
(select codDep as [@codDep],
  (select idCom as [@idCom], conteudoCom.query('
    myns:comunicado/*') from Comunicado where Comunicado.oficina = Oficina.codOfic AND Comunicado.departamento = Departamento.codDep
    for xml path ('myns:comunicado'), type) as [*]
  from Departamento where Departamento.oficina = Oficina.codOfic
  for xml path ('myns:departamento'), type) as [*]
 from Oficina for xml path ('myns:oficina'), root('myns:comunicados')
