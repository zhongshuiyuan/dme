--注册算法
insert into dme_algorithm
  (id, syscode, name, alias, version, createtime, remark, type, extension)
values
  (seq_dme_algorithm.nextval,
   '6e18c8abbf5448c7a4c15cfd4c6eed8c',
   'OverlayAnalysis',
   '两个要素类进行叠加分析',
   '1.0.0',
   1530023269000,
   '两个要素类进行叠加分析',
   'DLL',
   '[{"Key":"assembly","Value":"Dist.Dme.Algorithms.Overlay.dll","Desc":"DLL库名称"},{"Key":"mainClass","Value":"Dist.Dme.Algorithms.Overlay.OverlayMain","Desc":"主类名称"},{"Key":"mainMethod","Value":"","Desc":"主方法"},{"Key":"path","Value":"","Desc":"DLL路径"}]');

--注册算法参数
insert into dme_algorithm_meta(id, name, datatype, inout, algorithm_id, isvisible, remark, alias, readonly)
values(seq_dme_algorithm_meta.nextval, 'SourceFeatureClass', 17, );
--新建模型
insert into dme_model
values(seq_dme_model.nextval, '78cd4b737ec24a6796416231fdd3f867', '叠加分析模型', '叠加分析模型', 1530023269000);
commit;
--添加模型默认版本
insert into dme_model_version(id, syscode, name, model_id, createtime)
values(seq_dme_model_version.nextval, '4d523e8acaa84bd8823717dc1ae80dbf', 'DEFAULT', 21, 1530023269000);
commit;
--构建模型步骤
insert into dme_rulestep(id, syscode, model_id, version_id, gui_location_x, gui_location_y, step_type_id,step_name)
values(seq_dme_rulestep.nextval, 'd4f041935a2e459bb33742ff02815dde', 21, 41, 100, 200, 1, '算法输入');
--构建模型步骤参数
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'AlgorithmCode', '6e18c8abbf5448c7a4c15cfd4c6eed8c');
-->构建源要素类参数，指定数据源ID
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'SourceFeatureClass', '{"name":"TZFAFW","source":"30143df1123449a896429854899f37f3"}');
-->构建目标要素类参数，指定数据源ID
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'TargetFeatureClass', '{"name":"STKZXFW_YDFW","source":"791b05180d8c4e2186f7684ecf557457"}');
-->构建分析类型
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'AnalysisType', 0);






