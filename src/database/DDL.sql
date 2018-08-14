--������
CREATE TABLE DME_RuleStep
(
  "ID" NUMBER(8) NOT NULL,
   "SYSCODE" VARCHAR2(38),
  "MODEL_ID"  NUMBER(8),
  "ALGORITHM_ID"  NUMBER(8),
  "GUI_LOCATION_X" NUMBER(8, 3),
  "GUI_LOCATION_Y" NUMBER(8, 3),
  "STEP_TYPE_ID" NUMBER(8),
  "VERSION_ID" NUMBER(8)
);

-- Add comments to the table 
comment on table DME_RULESTEP
  is '������';
-- Add comments to the columns 
comment on column DME_RULESTEP.id
  is '����';
comment on column DME_RULESTEP.model_id
  is 'ģ��ID';
comment on column DME_RULESTEP.algorithm_id
  is '�㷨ID';
comment on column DME_RULESTEP.gui_location_x
  is '���滯����x';
comment on column DME_RULESTEP.gui_location_y
  is '���滯����y';
comment on column DME_RULESTEP.step_type_id
  is '��������ID';
comment on column DME_RULESTEP.version_id
  is '�汾ID';
comment on column DME_RULESTEP.syscode
  is 'ϵͳΨһ����';
  
  alter table DME_RULESTEP add primary key(id);
  create unique index UK_SYS_CODE on DME_RULESTEP (syscode);

CREATE TABLE DME_Task
(
	"ID" NUMBER(8),
	"SYSCODE" VARCHAR2(38),
	"NAME" VARCHAR2(50),
	"STATUS" VARCHAR2(20),
	"MODEL_ID" NUMBER(8),
	"VERSION_ID" NUMBER(8),
	"CREATETIME" number(20),
	"LASTTIME" number(20),
  REMARK VARCHAR2(250)
);
-- Add comments to the table 
comment on table DME_Task
  is '�������';
-- Add comments to the columns 
comment on column DME_Task.id
  is '����';
comment on column DME_Task.syscode
  is 'Ψһ����';
comment on column DME_Task.name
  is '����';
comment on column DME_Task.status
  is '״̬�������У�running��ֹͣ��stop���ɹ���success��ʧ�ܣ�fail
';
comment on column DME_Task.model_id
  is 'ģ��ID';
comment on column DME_Task.version_id
  is '�汾ID';
comment on column DME_Task.createtime
  is '����ʱ�䣬����';
comment on column DME_Task.lasttime
  is '������ʱ�䣬����';
comment on column DME_Task.REMARK
  is '��ע��Ϣ';
  
alter table DME_Task add primary key(id);

CREATE TABLE DME_Model_Version
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38),
	"NAME" VARCHAR2(50),
	"MODEL_ID" NUMBER(8),
	"CREATETIME" number(20)
);
-- Add comments to the table 
comment on table DME_MODEL_VERSION
  is 'ģ�Ͱ汾';
-- Add comments to the columns 
comment on column DME_MODEL_VERSION.id
  is '����';
comment on column DME_MODEL_VERSION.syscode
  is 'Ψһ����';
comment on column DME_MODEL_VERSION.name
  is '�汾����';
comment on column DME_MODEL_VERSION.model_id
  is 'ģ��ID';
comment on column DME_MODEL_VERSION.createtime
  is '����ʱ�䣬����';
  alter table DME_MODEL_VERSION add primary key(id);
  
  CREATE TABLE DME_Model
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"NAME" VARCHAR2(50) NOT NULL,
	"REMARK" VARCHAR2(250),
  "CREATETIME" number(20),
);
-- Add comments to the table 
comment on table DME_MODEL
  is 'ģ��';
-- Add comments to the columns 
comment on column DME_MODEL.id
  is '����ID';
comment on column DME_MODEL.syscode
  is 'Ψһ����';
comment on column DME_MODEL.name
  is '����';
comment on column DME_MODEL.remark
  is '��ע';
 comment on column DME_MODEL.CREATETIME
  is '����ʱ�䣬����';
  
  alter table DME_MODEL add primary key(id);
  
  CREATE TABLE DME_RuleStep_DataSource
(
	"ID" NUMBER(8) NOT NULL,
	"RULESTEP_ID" NUMBER(8),
	"MODEL_ID"  NUMBER(8),
  "VERSION_ID"  NUMBER(8),
	"DATASOURCE_ID"  NUMBER(8)
);
comment on table DME_RULESTEP_DATASOURCE
  is '���������������Դ';
comment on column DME_RULESTEP_DATASOURCE.id
  is '����';
comment on column DME_RULESTEP_DATASOURCE.rulestep_id
  is '������ID';
comment on column DME_RULESTEP_DATASOURCE.model_id
  is 'ģ��ID';
comment on column DME_RULESTEP_DATASOURCE.datasource_id
  is '����ԴID';
    alter table DME_RULESTEP_DATASOURCE add primary key(id);
  
  CREATE TABLE DME_RuleStep_Attribute
(
	"ID" NUMBER(8) NOT NULL,
	"RULESTEP_ID" NUMBER(8),
	"MODEL_ID" NUMBER(8),
  "VERSION_ID" NUMBER(8),
	"ATTRIBUTE_CODE" VARCHAR2(50),
	"ATTRIBUTE_VALUE" CLOB
);

-- Add comments to the table 
comment on table DME_RULESTEP_ATTRIBUTE
  is '�����������ֵ';
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.id
  is '����';
comment on column DME_RULESTEP_ATTRIBUTE.rulestep_id
  is '������ID';
comment on column DME_RULESTEP_ATTRIBUTE.model_id
  is 'ģ��ID';
comment on column DME_RULESTEP_ATTRIBUTE.version_id
  is '�汾ID';
comment on column DME_RULESTEP_ATTRIBUTE.attribute_code
  is '���Ա���';
comment on column DME_RULESTEP_ATTRIBUTE.attribute_value
  is '����ֵ';
  alter table DME_RULESTEP_ATTRIBUTE add primary key(ID);
  
  CREATE TABLE DME_Version
(
	"ID" NUMBER(8) NOT NULL,
	"MAJOR_VERSION" NUMBER(3),
	"MINOR_VERSION" NUMBER(3),
  "REVISION_VERSION" NUMBER(3),
	"UPGRADE_TIME" number(20)
);
-- Add comments to the table 
comment on table DME_VERSION
  is '��Ʒ�汾';
-- Add comments to the columns 
comment on column DME_VERSION.id
  is '����';
comment on column DME_VERSION.major_version
  is '���汾�����˲����ݵ� API �޸�';
comment on column DME_VERSION.minor_version
  is '�ΰ汾���������¼��ݵĹ���������';
 comment on column DME_VERSION.REVISION_VERSION
  is '�޶��ţ��������¼��ݵ���������';
comment on column DME_VERSION.UPGRADE_TIME
  is '����ʱ�䣬����';
  alter table DME_VERSION add primary key(id);
  
  CREATE TABLE DME_RuleStep_Hop
(
	"ID" NUMBER(8) NOT NULL,
	"MODEL_ID" NUMBER(8),
  "VERSION_ID" NUMBER(8),
	"STEP_FROM_ID" NUMBER(8),
	"SETP_TO_ID" NUMBER(8),
	"ENABLED" NUMBER(8) default 1
);
comment on table DME_RULESTEP_HOP
  is '������������Ϣ';
-- Add comments to the columns 
comment on column DME_RULESTEP_HOP.id
  is '����';
comment on column DME_RULESTEP_HOP.model_id
  is 'ģ��ID';
comment on column DME_RULESTEP_HOP.version_id
  is '�汾ID';
comment on column DME_RULESTEP_HOP.step_from_id
  is '��ʼ����ID';
comment on column DME_RULESTEP_HOP.setp_to_id
  is '��������ID';
comment on column DME_RULESTEP_HOP.enabled
  is '�Ƿ���ã�0�������ã�1������';
  
  alter table DME_RULESTEP_HOP add primary key(ID);
  
create table DME_LOG
(
  id         NUMBER(8) not null,
  logtype    VARCHAR2(20),
  loglevel   VARCHAR2(20),
  usercode   VARCHAR2(38),
  createtime number(20),
  address    VARCHAR2(38),
  remark     clob,
  apps       VARCHAR2(255),
  ObjectType VARCHAR2(30),
  ObjectId VARCHAR2(50)
)
tablespace DME
  pctfree 10
  initrans 1
  maxtrans 255;
-- Add comments to the table 
comment on table DME_LOG
  is '��־';
-- Add comments to the columns 
comment on column DME_LOG.id
  is '����';
comment on column DME_LOG.logtype
  is '��־���ͣ�LOGIN����¼����LOGOUT���ǳ���......';
comment on column DME_LOG.loglevel
  is '��־����ERROR��������־
MINIMAL����С��־
BASIC��������־
DETAILED����ϸ��־
DEBUG������';
comment on column DME_LOG.usercode
  is '�û�Ψһ����';
comment on column DME_LOG.createtime
  is '����ʱ�䣬����';
comment on column DME_LOG.address
  is '��ַ';
comment on column DME_LOG.remark
  is '��ע';
comment on column DME_LOG.apps
  is 'Ӧ��';
comment on column DME_LOG.ObjectType
  is '��־�������ͣ��磺�㷨��ģ�͵ȵ�';
comment on column DME_LOG.ObjectId
  is '����ֵ����¼����Ķ����ʶֵ';  
  
  alter table DME_LOG add primary key(ID);
  
  CREATE TABLE DME_DataSource_Type
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"CODE" VARCHAR2(50),
	"REMARK" VARCHAR2(255)
);
comment on table DME_DataSource_Type
  is '���ݿ�����';
-- Add comments to the columns 
comment on column DME_DataSource_Type.id
  is '����';
comment on column DME_DataSource_Type.syscode
  is 'Ψһ����';
comment on column DME_DataSource_Type.code
  is '����';
comment on column DME_DataSource_Type.remark
  is '��ע';

alter table DME_DataSource_Type add primary key(ID);
alter table DME_DATASOURCE_TYPE
  add constraint UK_DME_DATASOURCE_TYPE unique (CODE);

CREATE TABLE DME_Algorithm
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"NAME" VARCHAR2(50),
	"ALIAS" VARCHAR2(50),
	"VERSION" VARCHAR2(10) NOT NULL,
	"CREATETIME" number(20),
	"REMARK" VARCHAR2(250),
  TYPE VARCHAR2(10),
  EXTENSION CLOB
);
comment on table DME_ALGORITHM
  is '�㷨';
-- Add comments to the columns 
comment on column DME_ALGORITHM.id
  is '����';
comment on column DME_ALGORITHM.syscode
  is 'Ψһ����';
comment on column DME_ALGORITHM.name
  is 'Ψһ����';
comment on column DME_ALGORITHM.alias
  is '����';
comment on column DME_ALGORITHM.version
  is '�汾';
comment on column DME_ALGORITHM.CREATETIME
  is 'ע��ʱ�䣬����';
comment on column DME_ALGORITHM.remark
  is '��ע';
comment on column DME_ALGORITHM.type
  is '�㷨���ͣ��磺DLL��JAR��URI�ȵ�';
comment on column DME_ALGORITHM.EXTENSION
  is '��չ��Ϣ����ʽ��JSON�����type=DLL����洢DLL��assembly������.��������DLL·���ȵ���Ϣ';  
  
  alter table DME_ALGORITHM add primary key(ID);
  
  CREATE TABLE DME_Algorithm_Meta
(
	"ID" NUMBER(8) NOT NULL,
	"NAME" VARCHAR2(50),
	"CODE" VARCHAR2(30),
	"DATATYPE" NUMBER(2),
	"INOUT" VARCHAR2(5),
	"ALGORITHM_ID" NUMBER(8)
);
comment on table DME_Algorithm_Meta
  is '�㷨Ԫ����';
-- Add comments to the columns 
comment on column DME_Algorithm_Meta.id
  is '����';
comment on column DME_Algorithm_Meta.name
  is '��������';
comment on column DME_Algorithm_Meta.code
  is '��������';
comment on column DME_Algorithm_Meta.DATATYPE
  is '�����������ͣ�1���������ͣ�ValueMetaInterface.TYPE_NUMBER=1��
2���ַ������ͣ�ValueTypeMeta.TYPE_STRING=2��
3��ʱ�����ͣ�ValueTypeMeta.TYPE_DATE=3��
4���������ͣ�ValueTypeMeta.TYPE_BOOLEAN=4��
5���������ͣ�ValueTypeMeta.TYPE_INTEGER=5��
6�����������ͣ�ValueTypeMeta.TYPE_BIGNUMBER=6��
7�����л����ͣ�ValueTypeMeta.TYPE_SERIALIZABLE=7��
8�����������ͣ�ValueTypeMeta.TYPE_BINARY=8��
9��΢��ʱ�����ͣ�ValueTypeMeta.TYPE_TIMESTAMP=9��
10������·�����ͣ�ValueTypeMeta.TYPE_INET=10��
11�������ļ�·����ValueTypeMeta.TYPE_LOCAL_FILE=11��
12������mdb��Ҫ���ࣨValueTypeMeta.TYPE_MDB_FEATURECLASS=12��
13������gdb·����ValueTypeMeta.TYPE_GDB_PATH=13��
14��ʱ����루ValueTypeMeta.TYPE_MILLISECOND=14��';
comment on column DME_Algorithm_Meta.inout
  is '����������������룺IN�������OUT�������������ͣ�IN_F';
comment on column DME_Algorithm_Meta.algorithm_id
  is '�㷨ID';
  alter table DME_Algorithm_Meta add primary key(id);
  
  CREATE TABLE DME_DataSource
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"NAME" VARCHAR2(50) NOT NULL,
	"ISLOCAL" NUMBER(1) DEFAULT 0 NOT NULL,
	"TYPE" VARCHAR2(20) NOT NULL,
	"CONNECTION" CLOB,
	"CREATETIME" number(20),
	"REMARK" VARCHAR2(250)
);
comment on table DME_DATASOURCE
  is '����Դ';
-- Add comments to the columns 
comment on column DME_DATASOURCE.id
  is '����';
comment on column DME_DATASOURCE.syscode
  is 'Ψһ����';
comment on column DME_DATASOURCE.name
  is '����';
comment on column DME_DATASOURCE.islocal
  is '�Ƿ񱾵�����Դ��0��Զ�̣�1������';
comment on column DME_DATASOURCE.type
  is '����Դ���ͣ���д��ĸ��ʶ
���ISLOCAL=0����ʾԶ�̣���TYPE����Ϊ��ϵ�����ݿ⣺ORACLE��MYSQL��SQLSERVER��MONGODB����������·�����ݣ�MDB��GDB��EXCEL
���ISLOCAL=1����ʾ���أ���TYPE����Ϊ��MDB��GDB��EXCEL
';
comment on column DME_DATASOURCE.connection
  is '��������ӵ�ַ����Բ�ͬ���ͣ���ʽ��һ��
ʾ��������oracle
connectionֵ���Ǹ�json��ʽ
{
    "name":"bhoa",
    "server":"10.209.49.20",
    "type":"ORACLE",
    "database":"orcl",
    "port":1521,
    "username":"bhoa",
    "encrypted":1,
    "password":"2be98afc86aa7f2e4cb79ce10dc9aa0db"
}
';
comment on column DME_DATASOURCE.createtime
  is '����ʱ�䣬����';
comment on column DME_DATASOURCE.remark
  is '��ע';

alter table DME_DATASOURCE add primary key(ID);

-- Create table
create table DME_USER
(
  id         number(8),
  syscode    varchar2(38),
  loginname  varchar2(50),
  loginpwd   varchar2(50),
  name       varchar2(50),
  status     number(1),
  createtime number(20),
  email      varchar2(20),
  telephone  varchar2(20),
  usertype   number(1)
)
;
-- Add comments to the table 
comment on table DME_USER
  is '�û���Ϣ��';
-- Add comments to the columns 
comment on column DME_USER.id
  is '����';
comment on column DME_USER.syscode
  is 'Ψһ����';
comment on column DME_USER.loginname
  is '��¼��';
comment on column DME_USER.loginpwd
  is '��¼����';
comment on column DME_USER.name
  is '��ʾ����';
comment on column DME_USER.status
  is '�û�״̬��0����Ч��1����Ч��2����������3��ɾ��';
comment on column DME_USER.createtime
  is 'ע��ʱ�䣬����';
comment on column DME_USER.email
  is '����';
comment on column DME_USER.telephone
  is '��ϵ�绰';
comment on column DME_USER.usertype
  is '�û����͡�0�������û���1������Ա��2����ͨ�û�';
  
  alter table DME_USER add primary key(id);
  -- ����Ĭ��ֵ
alter table DME_USER modify status default 1;
alter table DME_USER modify usertype default 2;

-- Create sequence 
create sequence SEQ_DME_MODEL
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_ALGORITHM
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_ALGORITHM_META
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_DATASOURCE_TYPE
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_DATASOURCE
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_TASK
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_MODEL_VERSION
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_RULESTEP
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_RULESTEP_ATTRIBUTE
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_RULESTEP_DATASOURCE
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_RULESTEP_HOP
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_VERSION
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_USER
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_LOG
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

--��������
insert into DME_USER
values(seq_dme_user.nextval, lower(sys_guid()), 'dmeadmin','passw0rd', 'dme����Ա',1, SYSDATE, '754236623@qq.com','13917059080',1);

-- Add/modify columns 
alter table DME_ALGORITHM_META add isVisible integer default 0;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.isVisible
  is '�Ƿ�ɼ���0�����ɼ���1���ɼ�����������ģ�ͼ�������ʾ���������û��༭';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add remark varchar2(255);
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.remark
  is '��ע��Ϣ';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add alias varchar2(30);
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.alias
  is '����';
  
-- Add/modify columns 
alter table DME_ALGORITHM_META add readonly1 integer default 0;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.readonly
  is '�Ƿ�ֻ����1��ֻ����0���ɱ༭';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add REQUIRED integer default 1;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.REQUIRED
  is '�Ƿ���룬1�����룻0����ѡ';
  
  -- Add/modify columns 
alter table DME_RULESTEP add step_name varchar2(50);
-- Add comments to the columns 
comment on column DME_RULESTEP.step_name
  is '��������';

-- Create table
create table DME_RULESTEP_TYPE
(
  id     number(8),
  code   varchar2(50),
  name   varchar2(100),
  remark varchar2(255)
)
;
-- Add comments to the table 
comment on table DME_RULESTEP_TYPE
  is '����������';
-- Add comments to the columns 
comment on column DME_RULESTEP_TYPE.id
  is '����ID';
comment on column DME_RULESTEP_TYPE.code
  is '��������Ψһ����';
comment on column DME_RULESTEP_TYPE.name
  is '������������';
comment on column DME_RULESTEP_TYPE.remark
  is '�������ͱ�ע';
  
  alter table DME_RULESTEP_TYPE add primary key(id);
  create unique index UK_DME_RULESTEP_TYPE_CODE on DME_RULESTEP_TYPE (code);

  create sequence SEQ_DME_RULESTEP_TYPE
  minvalue 1
  maxvalue 9999999999999999999999999
  start with 1
  increment by 1
  cache 20;
  
  -- Add/modify columns 
alter table DME_RULESTEP_TYPE add CATEGORY varchar2(50);
-- Add comments to the columns 
comment on column DME_RULESTEP_TYPE.CATEGORY
  is '����';
  
  insert into DME_RULESTEP_TYPE 
values(SEQ_DME_RULESTEP_TYPE.NEXTVAL, 'AlgorithmInput', '�㷨����', 'ѡ����ע����㷨�������㷨����', '����');
  insert into DME_RULESTEP_TYPE 
values(SEQ_DME_RULESTEP_TYPE.NEXTVAL, 'DataSourceInput', '����Դ����', '����Դ����', '����');

-- ɾ��������ģ�͵��㷨id����
--���STEP_TYPE_ID=1������AlgorithmInput�����㷨��id��Ҫ��DME_RULESTEP_ATTRIBUTE�洢����code=algorithm_id��ʶ
alter table DME_RULESTEP drop column algorithm_id;

-- Create table
create table DME_Task_Result
(
  id          number(8) not null,
  task_ID   number(8),
  rulestep_id number(8),
  r_code      varchar2(100),
  r_type      varchar2(50),
  r_value     clob
)
;
-- Add comments to the table 
comment on table DME_Task_Result
  is '����������洢';
-- Add comments to the columns 
comment on column DME_Task_Result.id
  is '����ID';
comment on column DME_Task_Result.task_ID
  is '����ID';
comment on column DME_Task_Result.rulestep_id
  is '����ID';
comment on column DME_Task_Result.r_code
  is '�������';
comment on column DME_Task_Result.r_type
  is '������ͣ���ValueType������һ��';
comment on column DME_Task_Result.r_value
  is '������ֵ������R_TYPE�Ĳ�ͬ�����ֵҲ��ͬ';
  
  alter table dme_task_result add primary key(id);

-- Create sequence 
create sequence SEQ_DME_Task_Result
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

-- Create table
create table DME_DataSource_attribute
(
  id              number(8),
  datasource_id   number(8),
  attribute_code  varchar2(50),
  attribute_value clob
)
;
-- Add comments to the table 
comment on table DME_DataSource_attribute
  is '����Դ��չ����';
-- Add comments to the columns 
comment on column DME_DataSource_attribute.id
  is '����ID';
comment on column DME_DataSource_attribute.datasource_id
  is '����ԴID';
comment on column DME_DataSource_attribute.attribute_code
  is '���Ա���';
comment on column DME_DataSource_attribute.attribute_value
  is '����ֵ';
  
  alter table DME_DataSource_attribute add primary key(id);
create sequence SEQ_DME_DataSource_attribute
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;


-- Add/modify columns 
alter table DME_MODEL add ispublish inteGER default 0;
-- Add comments to the columns 
comment on column DME_MODEL.ispublish
  is '�Ƿ񷢲���0��δ������1������';

-- Add/modify columns 
alter table DME_MODEL add publishtime number(20);
-- Add comments to the columns 
comment on column DME_MODEL.publishtime
  is '����ʱ��';
  
  --��Ӽ�
  -- Create/Recreate primary, unique and foreign key constraints 
alter table DME_ALGORITHM
  add constraint UK_DME_ALGORITHM unique (SYSCODE);
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add required inteGER default 1;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.required
  is '�Ƿ���룬1�����룻0����ѡ';
  
  -- Add/modify columns 
alter table DME_RULESTEP_HOP add name varchar2(50);
-- Add comments to the columns 
comment on column DME_RULESTEP_HOP.name
  is '�����ߵ�����';
  
  
  -- ����ע�ͱ�
create table DME_NOTE
(
  id                     number(8) primary key,
  content                    CLOB,
  gui_location_x               INTEGER,
  gui_location_y               INTEGER,
  gui_location_width           INTEGER,
  gui_location_height          INTEGER,
  font_name                    VARCHAR2(255),
  font_size                    INTEGER,
  font_bold                    CHAR(1),
  font_italic                  CHAR(1),
  font_color_red               INTEGER,
  font_color_green             INTEGER,
  font_color_blue              INTEGER,
  font_back_ground_color_red   INTEGER,
  font_back_ground_color_green INTEGER,
  font_back_ground_color_blue  INTEGER,
  font_border_color_red        INTEGER,
  font_border_color_green      INTEGER,
  font_border_color_blue       INTEGER,
  draw_shadow                  INTEGER,
  model_id number(8),
  version_id number(8)
);
create sequence SEQ_DME_NOTE
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

-- ������� 
alter table DME_RULESTEP_ATTRIBUTE add isneedprecursor inteGER default 0;
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.isneedprecursor
  is '�Ƿ���Ҫǰ������ʾ������������Ĳ����������1����attribute_value��ʽ��${�������:��������}';
  
  -- Add/modify columns 
alter table DME_RULESTEP rename column step_name to NAME;
alter table DME_RULESTEP add remark varchar2(255);
-- Add comments to the columns 
comment on column DME_RULESTEP.remark
  is '���豸ע��Ϣ';

-- Add/modify columns 
alter table DME_TASK add nodeserver varchar2(50);
-- Add comments to the columns 
comment on column DME_TASK.nodeserver
  is '�ڵ������';

-- 20180715
alter table DME_TASK_RESULT add status varchar2(20);
-- Add comments to the columns 
comment on column DME_TASK_RESULT.status
  is '״̬�������У�running��ֹͣ��stop���ɹ���success��ʧ�ܣ�fail';
  
  --20180716 �������Դ����
  insert into DME_DATASOURCE_TYPE
values(seq_dme_datasource_type.nextval, 'ef2ad4ffbe4d466cb11e0edc97321d75', 'MONGODB', 'mongodb')

insert into DME_RULESTEP_TYPE
values(seq_dme_rulestep_type.nextval, 'MongodbOutput', 'mongo���', 'mongo���', '���');
--��������Դ-mongo
insert into DME_DATASOURCE
values(seq_dme_datasource.nextval, '5655aa2d80e5474cadd7ba9cb7c57d15', '�ļ����ݿ�', 'MONGODB', '{
      "Connection": "mongodb://192.168.1.67:27017",
      "DataBase": "DME",
      "userName":"",
      "password":""
    }', 1531735840000, '�ǽṹ���ݴ洢');
    
    insert into DME_RULESTEP
values(seq_dme_rulestep.nextval, '6a26dff0f14a42a2a07d1f7fc757722f', 21, 234,456, 41, 41, '���', 'mongo���');

insert into DME_RULESTEP_HOP 
values(seq_dme_rulestep_hop.nextval, 21, 41, 41, 85, 1, '���');

insert into DME_RULESTEP_ATTRIBUTE 
values(seq_dme_rulestep_attribute.nextval, 85, 21, 41, 'Database', 'DME', 0);

create table DME_TASK_RuleSTEP
(
  id          number(8),
  syscode VARCHAR2(38),
  task_id     number(8),
  rulestep_id number(8),
  status      varchar2(20),
  createtime  number(20),
  lastTime number(20)
)
;
-- Add comments to the table 
comment on table DME_TASK_RuleSTEP
  is '����������������ϵ';
-- Add comments to the columns 
comment on column DME_TASK_RuleSTEP.syscode
  is 'ϵͳΨһ����';
comment on column DME_TASK_RuleSTEP.id
  is '����id';
comment on column DME_TASK_RuleSTEP.task_id
  is '����id';
comment on column DME_TASK_RuleSTEP.rulestep_id
  is '������id';
comment on column DME_TASK_RuleSTEP.status
  is '״̬�������У�running��ֹͣ��stop���ɹ���success��ʧ�ܣ�fail';
comment on column DME_TASK_RuleSTEP.createtime
  is '����ʱ�䣬����';
 comment on column DME_TASK_RuleSTEP.Lasttime
  is '����޸�ʱ�䣬����';
  
alter table DME_TASK_RuleSTEP add primary key(id);

create sequence SEQ_DME_TASK_RuleSTEP
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

-- ���ģ�ͷ���
alter table DME_MODEL add category varchar2(50);
-- Add comments to the columns 
comment on column DME_MODEL.category
  is '����';
  
  -- Add/modify columns 
alter table DME_RULESTEP_ATTRIBUTE add rowindex inteGER default 0;
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.rowindex
  is '�кţ�Ĭ��Ϊ0�����ڱ�ʶ����������';
  
  create table DME_MODEL_IMG
(
  id         number(8),
  model_id   number(8),
  version_id number(8),
  imgcode    varchar2(38) not null,
  suffix varchar2(20),
  SOURCENAME varchar2(100),
  contentType VARCHAR2(50)
)
;
-- Add comments to the table 
comment on table DME_MODEL_IMG
  is 'ģ��ͼƬ';
-- Add comments to the columns 
comment on column DME_MODEL_IMG.id
  is '����';
comment on column DME_MODEL_IMG.model_id
  is 'ģ��id';
comment on column DME_MODEL_IMG.version_id
  is '�汾id';
comment on column DME_MODEL_IMG.imgcode
  is 'ͼƬΨһ����';
  comment on column DME_MODEL_IMG.suffix
  is '��׺';
  comment on column DME_MODEL_IMG.SOURCENAME
  is 'Դ�ļ���';
  comment on column DME_MODEL_IMG.contentType
  is '����';
  alter table DME_MODEL_IMG add primary key(id);
  
  create sequence SEQ_DME_MODEL_IMG
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

-- Add/modify columns 
alter table DME_MODEL add status integer default 1;
-- Add comments to the columns 
comment on column DME_MODEL.status
  is '״̬��0��ɾ����1������';

-- Add/modify columns 
alter table DME_MODEL_VERSION add status integer default 1;
-- Add comments to the columns 
comment on column DME_MODEL_VERSION.status
  is '״̬��0��ɾ����1������';
  
  
  -- Create table
create table DME_MODEL_TYPE
(
  id         number(8),
  syscode    varchar2(38),
  name       varchar2(50),
  createtime number(20),
  lasttime   number(20)
)
;
-- Add comments to the table 
comment on table DME_MODEL_TYPE
  is 'ģ������';
-- Add comments to the columns 
comment on column DME_MODEL_TYPE.id
  is '����ID';
comment on column DME_MODEL_TYPE.syscode
  is 'Ψһ����';
comment on column DME_MODEL_TYPE.name
  is '����';
comment on column DME_MODEL_TYPE.createtime
  is '����ʱ��';
comment on column DME_MODEL_TYPE.lasttime
  is '�޸�ʱ��';
  alter table DME_MODEL_TYPE add primary key(id);
  
create sequence SEQ_DME_MODEL_TYPE
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

-- Add/modify columns 
alter table DME_MODEL add model_type_id number(8);
-- Add comments to the columns 
comment on column DME_MODEL.model_type_id
  is 'ģ������ID';
alter table DME_MODEL add model_type_code varchar2(38);
-- Add comments to the columns 
comment on column DME_MODEL.model_type_code
  is 'ģ�����ͱ���';
  
insert into DME_DATASOURCE_TYPE
values
  (seq_dme_datasource_type.nextval,
   '5750049705814c6cb940d0f4c19920cf',
   'DME_FILESYSTEM',
   'dme�ļ�ϵͳ����Ҫ�洢�ڷֲ�ʽ�ļ�ϵͳ');
  commit;
  
     -- Add/modify columns 
alter table DME_RULESTEP_ATTRIBUTE add attribute_type integer default 0;
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.attribute_type
  is '�������0��һ�����ԣ�1������ʱ���ԣ���ζģ������ʱ��Ҫָ����';
  
  --��������
  create index idx_dme_model on dme_model(syscode);
  create index idx_dme_model_type on dme_model_type(syscode);
  create index idx_DME_RULESTEP on DME_RULESTEP(syscode);

  
  



  
