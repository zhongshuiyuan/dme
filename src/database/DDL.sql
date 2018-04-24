--规则步骤
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
  is '规则步骤';
-- Add comments to the columns 
comment on column DME_RULESTEP.id
  is '主键';
comment on column DME_RULESTEP.model_id
  is '模型ID';
comment on column DME_RULESTEP.algorithm_id
  is '算法ID';
comment on column DME_RULESTEP.gui_location_x
  is '界面化坐标x';
comment on column DME_RULESTEP.gui_location_y
  is '界面化坐标y';
comment on column DME_RULESTEP.step_type_id
  is '步骤类型ID';
comment on column DME_RULESTEP.version_id
  is '版本ID';
comment on column DME_RULESTEP.syscode
  is '系统唯一编码';
  
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
  is '任务调度';
-- Add comments to the columns 
comment on column DME_JOB.id
  is '主键';
comment on column DME_JOB.syscode
  is '唯一编码';
comment on column DME_JOB.name
  is '名称';
comment on column DME_JOB.status
  is '状态，运行中：running，停止：stop，成功：success，失败：fail
';
comment on column DME_JOB.model_id
  is '模型ID';
comment on column DME_JOB.version_id
  is '版本ID';
comment on column DME_JOB.createtime
  is '创建时间，毫秒';
comment on column DME_JOB.lasttime
  is '最后更新时间，毫秒';
comment on column DME_JOB.usercode
  is '用户唯一编码';
  
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
  is '模型版本';
-- Add comments to the columns 
comment on column DME_MODEL_VERSION.id
  is '主键';
comment on column DME_MODEL_VERSION.syscode
  is '唯一编码';
comment on column DME_MODEL_VERSION.name
  is '版本名称';
comment on column DME_MODEL_VERSION.model_id
  is '模型ID';
comment on column DME_MODEL_VERSION.createtime
  is '创建时间，毫秒';
comment on column DME_MODEL_VERSION.usercode
  is '用户唯一编码';
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
  is '模型';
-- Add comments to the columns 
comment on column DME_MODEL.id
  is '主键ID';
comment on column DME_MODEL.syscode
  is '唯一编码';
comment on column DME_MODEL.name
  is '名称';
comment on column DME_MODEL.remark
  is '备注';
comment on column DME_MODEL.usercode
  is '用户唯一编码';
  
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
  is '规则步骤关联的数据源';
comment on column DME_RULESTEP_DATASOURCE.id
  is '主键';
comment on column DME_RULESTEP_DATASOURCE.rulestep_id
  is '规则步骤ID';
comment on column DME_RULESTEP_DATASOURCE.model_id
  is '模型ID';
comment on column DME_RULESTEP_DATASOURCE.datasource_id
  is '数据源ID';
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
  is '规则步骤的属性值';
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.id
  is '主键';
comment on column DME_RULESTEP_ATTRIBUTE.rulestep_id
  is '规则步骤ID';
comment on column DME_RULESTEP_ATTRIBUTE.model_id
  is '模型ID';
comment on column DME_RULESTEP_ATTRIBUTE.version_id
  is '版本ID';
comment on column DME_RULESTEP_ATTRIBUTE.attribute_code
  is '属性编码';
comment on column DME_RULESTEP_ATTRIBUTE.attribute_value
  is '属性值';
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
  is '产品版本';
-- Add comments to the columns 
comment on column DME_VERSION.id
  is '主键';
comment on column DME_VERSION.major_version
  is '主版本';
comment on column DME_VERSION.minor_version
  is '次版本';
comment on column DME_VERSION.UPGRADE_TIME
  is '更新时间，毫秒';
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
  is '规则步骤流程信息';
-- Add comments to the columns 
comment on column DME_RULESTEP_HOP.id
  is '主键';
comment on column DME_RULESTEP_HOP.model_id
  is '模型ID';
comment on column DME_RULESTEP_HOP.version_id
  is '版本ID';
comment on column DME_RULESTEP_HOP.step_from_id
  is '开始步骤ID';
comment on column DME_RULESTEP_HOP.setp_to_id
  is '结束步骤ID';
comment on column DME_RULESTEP_HOP.enabled
  is '是否可用，0：不可用；1：可用';
  
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
  is '日志';
-- Add comments to the columns 
comment on column DME_LOG.id
  is '主键';
comment on column DME_LOG.logtype
  is '日志类型，LOGIN（登录）、LOGOUT（登出）......';
comment on column DME_LOG.loglevel
  is '日志级别，ERROR：错误日志
MINIMAL：最小日志
BASIC：基本日志
DETAILED：详细日志
DEBUG：调试';
comment on column DME_LOG.usercode
  is '用户唯一编码';
comment on column DME_LOG.createtime
  is '创建时间，毫秒';
comment on column DME_LOG.address
  is '地址';
comment on column DME_LOG.remark
  is '备注';
comment on column DME_LOG.apps
  is '应用';
  alter table DME_LOG add primary key(ID);
  
  CREATE TABLE DME_DatabaseType
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"CODE" VARCHAR2(50),
	"REMARK" VARCHAR2(255)
);
comment on table DME_DATABASETYPE
  is '数据库类型';
-- Add comments to the columns 
comment on column DME_DATABASETYPE.id
  is '主键';
comment on column DME_DATABASETYPE.syscode
  is '唯一编码';
comment on column DME_DATABASETYPE.code
  is '编码';
comment on column DME_DATABASETYPE.remark
  is '备注';

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
  is '算法';
-- Add comments to the columns 
comment on column DME_ALGORITHM.id
  is '主键';
comment on column DME_ALGORITHM.syscode
  is '唯一编码';
comment on column DME_ALGORITHM.name
  is '唯一名称';
comment on column DME_ALGORITHM.alias
  is '别名';
comment on column DME_ALGORITHM.version
  is '版本';
comment on column DME_ALGORITHM.registertime
  is '注册时间，毫秒';
comment on column DME_ALGORITHM.remark
  is '备注';
comment on column DME_ALGORITHM.usercode
  is '用户唯一编码';
comment on column DME_ALGORITHM.path
  is '路径';
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
  is '算法元数据';
-- Add comments to the columns 
comment on column DME_Algorithm_Meta.id
  is '主键';
comment on column DME_Algorithm_Meta.name
  is '参数名称';
comment on column DME_Algorithm_Meta.code
  is '参数编码';
comment on column DME_Algorithm_Meta.type
  is '参数类型，1、数字类型（ValueMetaInterface.TYPE_NUMBER=1）
2、字符串类型（ValueMetaInterface.TYPE_STRING=2）
3、时间类型（ValueMetaInterface.TYPE_DATE=3）
4、布尔类型（ValueMetaInterface.TYPE_BOOLEAN=4）
5、整型类型（ValueMetaInterface.TYPE_INTEGER=5）
6、大整型类型（ValueMetaInterface.TYPE_BIGNUMBER=6）
7、序列化类型（ValueMetaInterface.TYPE_SERIALIZABLE=7）
8、二进制类型（ValueMetaInterface.TYPE_BINARY=8）
9、微秒时间类型（ValueMetaInterface.TYPE_TIMESTAMP=9）
10、网络路径类型（ValueMetaInterface.TYPE_INET=10）';
comment on column DME_Algorithm_Meta.inout
  is '输入输出参数，输入：IN；输出：OUT';
comment on column DME_Algorithm_Meta.algorithm_id
  is '算法ID';
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
  is '数据源';
-- Add comments to the columns 
comment on column DME_DATASOURCE.id
  is '主键';
comment on column DME_DATASOURCE.syscode
  is '唯一编码';
comment on column DME_DATASOURCE.name
  is '名称';
comment on column DME_DATASOURCE.islocal
  is '是否本地数据源，0：远程；1：本地';
comment on column DME_DATASOURCE.type
  is '数据源类型，大写字母标识
如果ISLOCAL=0，表示远程，则TYPE可以为关系型数据库：ORACLE、MYSQL、SQLSERVER；或者网络路径数据：MDB、GDB、EXCEL
如果ISLOCAL=1，表示本地，则TYPE可以为：MDB、GDB、EXCEL
';
comment on column DME_DATASOURCE.connection
  is '具体的连接地址，针对不同类型，格式不一样
示例：例如oracle
connection值，是个json格式
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
  is '创建时间，毫秒';
comment on column DME_DATASOURCE.remark
  is '备注';

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
  is '用户信息表';
-- Add comments to the columns 
comment on column DME_USER.id
  is '主键';
comment on column DME_USER.syscode
  is '唯一编码';
comment on column DME_USER.loginname
  is '登录名';
comment on column DME_USER.loginpwd
  is '登录密码';
comment on column DME_USER.name
  is '显示名称';
comment on column DME_USER.status
  is '用户状态，0：无效；1：有效；2：不正常；3：删除';
comment on column DME_USER.createtime
  is '注册时间，毫秒';
comment on column DME_USER.email
  is '邮箱';
comment on column DME_USER.telephone
  is '联系电话';
comment on column DME_USER.usertype
  is '用户类型。0：内置用户；1：管理员；2：普通用户';
  
  alter table DME_USER add primary key(id);
  -- 设置默认值
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

--测试数据
insert into DME_USER
values(seq_dme_user.nextval, lower(sys_guid()), 'dmeadmin','passw0rd', 'dme管理员',1, SYSDATE, '754236623@qq.com','13917059080',1);

-- Add/modify columns 
alter table DME_ALGORITHM_META add isVisible integer default 0;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.isVisible
  is '是否可见，0：不可见；1：可见。可用于在模型级别上显示，供最终用户编辑';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add remark varchar2(50);
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.remark
  is '备注信息';









  
