--ע���㷨
insert into dme_algorithm
  (id, syscode, name, alias, version, createtime, remark, type, extension)
values
  (seq_dme_algorithm.nextval,
   '6e18c8abbf5448c7a4c15cfd4c6eed8c',
   'OverlayAnalysis',
   '����Ҫ������е��ӷ���',
   '1.0.0',
   1530023269000,
   '����Ҫ������е��ӷ���',
   'DLL',
   '[{"Key":"assembly","Value":"Dist.Dme.Algorithms.Overlay.dll","Desc":"DLL������"},{"Key":"mainClass","Value":"Dist.Dme.Algorithms.Overlay.OverlayMain","Desc":"��������"},{"Key":"mainMethod","Value":"","Desc":"������"},{"Key":"path","Value":"","Desc":"DLL·��"}]');

--ע���㷨����
insert into dme_algorithm_meta(id, name, datatype, inout, algorithm_id, isvisible, remark, alias, readonly)
values(seq_dme_algorithm_meta.nextval, 'SourceFeatureClass', 17, );
--�½�ģ��
insert into dme_model
values(seq_dme_model.nextval, '78cd4b737ec24a6796416231fdd3f867', '���ӷ���ģ��', '���ӷ���ģ��', 1530023269000);
commit;
--���ģ��Ĭ�ϰ汾
insert into dme_model_version(id, syscode, name, model_id, createtime)
values(seq_dme_model_version.nextval, '4d523e8acaa84bd8823717dc1ae80dbf', 'DEFAULT', 21, 1530023269000);
commit;
--����ģ�Ͳ���
insert into dme_rulestep(id, syscode, model_id, version_id, gui_location_x, gui_location_y, step_type_id,step_name)
values(seq_dme_rulestep.nextval, 'd4f041935a2e459bb33742ff02815dde', 21, 41, 100, 200, 1, '�㷨����');
--����ģ�Ͳ������
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'AlgorithmCode', '6e18c8abbf5448c7a4c15cfd4c6eed8c');
-->����ԴҪ���������ָ������ԴID
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'SourceFeatureClass', '{"name":"TZFAFW","source":"30143df1123449a896429854899f37f3"}');
-->����Ŀ��Ҫ���������ָ������ԴID
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'TargetFeatureClass', '{"name":"STKZXFW_YDFW","source":"791b05180d8c4e2186f7684ecf557457"}');
-->������������
insert into dme_rulestep_attribute(id, rulestep_id, model_id, version_id, attribute_code, attribute_value)
values(seq_dme_rulestep_attribute.nextval, 21, 21, 41, 'AnalysisType', 0);






