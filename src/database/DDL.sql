--������
CREATE TABLE DME_RuleStep
(
  "ID" NUMBER(8) NOT NULL,
   "SYSCODE" VARCHAR2(38),
  "MODEL_ID"  NUMBER(8),
  "ALGORITHM_ID"  NUMBER(8),
  "GUI_LOCATION_X" NUMBER(8),
  "GUI_LOCATION_Y" NUMBER(8),
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
	"USERCODE" VARCHAR2(38)
);
-- Add comments to the table 
comment on table DME_Task
  is '�������';
-- Add comments to the columns 
comment on column DME_JOB.id
  is '����';
comment on column DME_JOB.syscode
  is 'Ψһ����';
comment on column DME_JOB.name
  is '����';
comment on column DME_JOB.status
  is '״̬�������У�running��ֹͣ��stop���ɹ���success��ʧ�ܣ�fail
';
comment on column DME_JOB.model_id
  is 'ģ��ID';
comment on column DME_JOB.version_id
  is '�汾ID';
comment on column DME_JOB.createtime
  is '����ʱ�䣬����';
comment on column DME_JOB.lasttime
  is '������ʱ�䣬����';
comment on column DME_JOB.usercode
  is '�û�Ψһ����';
  
alter table DME_JOB add primary key(id);

CREATE TABLE DME_Model_Version
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38),
	"NAME" VARCHAR2(50),
	"MODEL_ID" NUMBER(8),
	"CREATETIME" number(20),
	"USERCODE" VARCHAR2(38)
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
comment on column DME_MODEL_VERSION.usercode
  is '�û�Ψһ����';
  alter table DME_MODEL_VERSION add primary key(id);
  
  CREATE TABLE DME_Model
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"NAME" VARCHAR2(50) NOT NULL,
	"REMARK" VARCHAR2(250),
	"USERCODE" VARCHAR2(38)
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
comment on column DME_MODEL.usercode
  is '�û�Ψһ����';
  
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
	"MAJOR_VERSION" NUMBER(2),
	"MINOR_VERSION" NUMBER(2),
	"UPGRADE_TIME" number(20)
);
-- Add comments to the table 
comment on table DME_VERSION
  is '��Ʒ�汾';
-- Add comments to the columns 
comment on column DME_VERSION.id
  is '����';
comment on column DME_VERSION.major_version
  is '���汾';
comment on column DME_VERSION.minor_version
  is '�ΰ汾';
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
	"ENABLED" NUMBER(8)
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
  remark     VARCHAR2(255),
  apps       VARCHAR2(255)
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
  alter table DME_LOG add primary key(ID);
  
  CREATE TABLE DME_DatabaseType
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"CODE" VARCHAR2(50),
	"REMARK" VARCHAR2(255)
);
comment on table DME_DATABASETYPE
  is '���ݿ�����';
-- Add comments to the columns 
comment on column DME_DATABASETYPE.id
  is '����';
comment on column DME_DATABASETYPE.syscode
  is 'Ψһ����';
comment on column DME_DATABASETYPE.code
  is '����';
comment on column DME_DATABASETYPE.remark
  is '��ע';

alter table DME_DATABASETYPE add primary key(ID);


CREATE TABLE DME_Algorithm
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"NAME" VARCHAR2(50),
	"ALIAS" VARCHAR2(50),
	"VERSION" VARCHAR2(10) NOT NULL,
	"REGISTERTIME" number(20),
	"REMARK" VARCHAR2(250),
	"USERCODE" VARCHAR2(38),
  PATH VARCHAR2(512)
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
comment on column DME_ALGORITHM.registertime
  is 'ע��ʱ�䣬����';
comment on column DME_ALGORITHM.remark
  is '��ע';
comment on column DME_ALGORITHM.usercode
  is '�û�Ψһ����';
comment on column DME_ALGORITHM.path
  is '·��';
  alter table DME_ALGORITHM add primary key(ID);
  
  CREATE TABLE DME_Algorithm_Meta
(
	"ID" NUMBER(8) NOT NULL,
	"NAME" VARCHAR2(15),
	"CODE" VARCHAR2(30),
	"TYPE" NUMBER(1),
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
comment on column DME_Algorithm_Meta.type
  is '�������ͣ�1���������ͣ�ValueMetaInterface.TYPE_NUMBER=1��
2���ַ������ͣ�ValueMetaInterface.TYPE_STRING=2��
3��ʱ�����ͣ�ValueMetaInterface.TYPE_DATE=3��
4���������ͣ�ValueMetaInterface.TYPE_BOOLEAN=4��
5���������ͣ�ValueMetaInterface.TYPE_INTEGER=5��
6�����������ͣ�ValueMetaInterface.TYPE_BIGNUMBER=6��
7�����л����ͣ�ValueMetaInterface.TYPE_SERIALIZABLE=7��
8�����������ͣ�ValueMetaInterface.TYPE_BINARY=8��
9��΢��ʱ�����ͣ�ValueMetaInterface.TYPE_TIMESTAMP=9��
10������·�����ͣ�ValueMetaInterface.TYPE_INET=10��';
comment on column DME_Algorithm_Meta.inout
  is '����������������룺IN�������OUT';
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
���ISLOCAL=0����ʾԶ�̣���TYPE����Ϊ��ϵ�����ݿ⣺ORACLE��MYSQL��SQLSERVER����������·�����ݣ�MDB��GDB��EXCEL
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
    "password":"Encrypted 2be98afc86aa7f2e4cb79ce10dc9aa0db"
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

create sequence SEQ_DME_ALGORITHM_METADATA
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

create sequence SEQ_DME_DATABASETYPE
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

--��������
insert into DME_USER
values(seq_dme_user.nextval, lower(sys_guid()), 'dmeadmin','passw0rd', 'dme����Ա',1, SYSDATE, '754236623@qq.com','13917059080',1);

-- Add/modify columns 
alter table DME_ALGORITHM_META add isVisible integer default 0;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.isVisible
  is '�Ƿ�ɼ���0�����ɼ���1���ɼ�����������ģ�ͼ�������ʾ���������û��༭';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add remark varchar2(50);
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.remark
  is '��ע��Ϣ';









  
