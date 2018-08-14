--规则步骤
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
  REMARK VARCHAR2(250)
);
-- Add comments to the table 
comment on table DME_Task
  is '任务调度';
-- Add comments to the columns 
comment on column DME_Task.id
  is '主键';
comment on column DME_Task.syscode
  is '唯一编码';
comment on column DME_Task.name
  is '名称';
comment on column DME_Task.status
  is '状态，运行中：running，停止：stop，成功：success，失败：fail
';
comment on column DME_Task.model_id
  is '模型ID';
comment on column DME_Task.version_id
  is '版本ID';
comment on column DME_Task.createtime
  is '创建时间，毫秒';
comment on column DME_Task.lasttime
  is '最后更新时间，毫秒';
comment on column DME_Task.REMARK
  is '备注信息';
  
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
 comment on column DME_MODEL.CREATETIME
  is '创建时间，毫秒';
  
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
	"MAJOR_VERSION" NUMBER(3),
	"MINOR_VERSION" NUMBER(3),
  "REVISION_VERSION" NUMBER(3),
	"UPGRADE_TIME" number(20)
);
-- Add comments to the table 
comment on table DME_VERSION
  is '产品版本';
-- Add comments to the columns 
comment on column DME_VERSION.id
  is '主键';
comment on column DME_VERSION.major_version
  is '主版本，做了不兼容的 API 修改';
comment on column DME_VERSION.minor_version
  is '次版本，做了向下兼容的功能性新增';
 comment on column DME_VERSION.REVISION_VERSION
  is '修订号，做了向下兼容的问题修正';
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
	"ENABLED" NUMBER(8) default 1
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
comment on column DME_LOG.ObjectType
  is '日志对象类型，如：算法、模型等等';
comment on column DME_LOG.ObjectId
  is '对象值，记录具体的对象标识值';  
  
  alter table DME_LOG add primary key(ID);
  
  CREATE TABLE DME_DataSource_Type
(
	"ID" NUMBER(8) NOT NULL,
	"SYSCODE" VARCHAR2(38) NOT NULL,
	"CODE" VARCHAR2(50),
	"REMARK" VARCHAR2(255)
);
comment on table DME_DataSource_Type
  is '数据库类型';
-- Add comments to the columns 
comment on column DME_DataSource_Type.id
  is '主键';
comment on column DME_DataSource_Type.syscode
  is '唯一编码';
comment on column DME_DataSource_Type.code
  is '编码';
comment on column DME_DataSource_Type.remark
  is '备注';

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
comment on column DME_ALGORITHM.CREATETIME
  is '注册时间，毫秒';
comment on column DME_ALGORITHM.remark
  is '备注';
comment on column DME_ALGORITHM.type
  is '算法类型，如：DLL，JAR，URI等等';
comment on column DME_ALGORITHM.EXTENSION
  is '扩展信息，格式：JSON。如果type=DLL，则存储DLL的assembly，主类.主方法，DLL路径等等信息';  
  
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
  is '算法元数据';
-- Add comments to the columns 
comment on column DME_Algorithm_Meta.id
  is '主键';
comment on column DME_Algorithm_Meta.name
  is '参数名称';
comment on column DME_Algorithm_Meta.code
  is '参数编码';
comment on column DME_Algorithm_Meta.DATATYPE
  is '参数数据类型，1、数字类型（ValueMetaInterface.TYPE_NUMBER=1）
2、字符串类型（ValueTypeMeta.TYPE_STRING=2）
3、时间类型（ValueTypeMeta.TYPE_DATE=3）
4、布尔类型（ValueTypeMeta.TYPE_BOOLEAN=4）
5、整型类型（ValueTypeMeta.TYPE_INTEGER=5）
6、大整型类型（ValueTypeMeta.TYPE_BIGNUMBER=6）
7、序列化类型（ValueTypeMeta.TYPE_SERIALIZABLE=7）
8、二进制类型（ValueTypeMeta.TYPE_BINARY=8）
9、微秒时间类型（ValueTypeMeta.TYPE_TIMESTAMP=9）
10、网络路径类型（ValueTypeMeta.TYPE_INET=10）
11、本地文件路径（ValueTypeMeta.TYPE_LOCAL_FILE=11）
12、本地mdb的要素类（ValueTypeMeta.TYPE_MDB_FEATURECLASS=12）
13、本地gdb路径（ValueTypeMeta.TYPE_GDB_PATH=13）
14、时间毫秒（ValueTypeMeta.TYPE_MILLISECOND=14）';
comment on column DME_Algorithm_Meta.inout
  is '输入输出参数，输入：IN；输出：OUT；特征参数类型：IN_F';
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
如果ISLOCAL=0，表示远程，则TYPE可以为关系型数据库：ORACLE、MYSQL、SQLSERVER、MONGODB；或者网络路径数据：MDB、GDB、EXCEL
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
    "encrypted":1,
    "password":"2be98afc86aa7f2e4cb79ce10dc9aa0db"
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

--测试数据
insert into DME_USER
values(seq_dme_user.nextval, lower(sys_guid()), 'dmeadmin','passw0rd', 'dme管理员',1, SYSDATE, '754236623@qq.com','13917059080',1);

-- Add/modify columns 
alter table DME_ALGORITHM_META add isVisible integer default 0;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.isVisible
  is '是否可见，0：不可见；1：可见。可用于在模型级别上显示，供最终用户编辑';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add remark varchar2(255);
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.remark
  is '备注信息';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add alias varchar2(30);
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.alias
  is '别名';
  
-- Add/modify columns 
alter table DME_ALGORITHM_META add readonly1 integer default 0;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.readonly
  is '是否只读，1：只读；0：可编辑';
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add REQUIRED integer default 1;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.REQUIRED
  is '是否必须，1：必须；0：可选';
  
  -- Add/modify columns 
alter table DME_RULESTEP add step_name varchar2(50);
-- Add comments to the columns 
comment on column DME_RULESTEP.step_name
  is '步骤名称';

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
  is '规则步骤类型';
-- Add comments to the columns 
comment on column DME_RULESTEP_TYPE.id
  is '主键ID';
comment on column DME_RULESTEP_TYPE.code
  is '步骤类型唯一代码';
comment on column DME_RULESTEP_TYPE.name
  is '步骤类型名称';
comment on column DME_RULESTEP_TYPE.remark
  is '步骤类型备注';
  
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
  is '分组';
  
  insert into DME_RULESTEP_TYPE 
values(SEQ_DME_RULESTEP_TYPE.NEXTVAL, 'AlgorithmInput', '算法输入', '选择已注册的算法，配置算法参数', '输入');
  insert into DME_RULESTEP_TYPE 
values(SEQ_DME_RULESTEP_TYPE.NEXTVAL, 'DataSourceInput', '数据源输入', '数据源输入', '输入');

-- 删除规则步骤模型的算法id属性
--如果STEP_TYPE_ID=1，即：AlgorithmInput，则算法的id需要在DME_RULESTEP_ATTRIBUTE存储，以code=algorithm_id标识
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
  is '任务计算结果存储';
-- Add comments to the columns 
comment on column DME_Task_Result.id
  is '主键ID';
comment on column DME_Task_Result.task_ID
  is '任务ID';
comment on column DME_Task_Result.rulestep_id
  is '步骤ID';
comment on column DME_Task_Result.r_code
  is '结果编码';
comment on column DME_Task_Result.r_type
  is '结果类型，跟ValueType的名称一致';
comment on column DME_Task_Result.r_value
  is '结果输出值，根据R_TYPE的不同，输出值也不同';
  
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
  is '数据源扩展属性';
-- Add comments to the columns 
comment on column DME_DataSource_attribute.id
  is '主键ID';
comment on column DME_DataSource_attribute.datasource_id
  is '数据源ID';
comment on column DME_DataSource_attribute.attribute_code
  is '属性编码';
comment on column DME_DataSource_attribute.attribute_value
  is '属性值';
  
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
  is '是否发布。0：未发布；1：发布';

-- Add/modify columns 
alter table DME_MODEL add publishtime number(20);
-- Add comments to the columns 
comment on column DME_MODEL.publishtime
  is '发布时间';
  
  --添加键
  -- Create/Recreate primary, unique and foreign key constraints 
alter table DME_ALGORITHM
  add constraint UK_DME_ALGORITHM unique (SYSCODE);
  
  -- Add/modify columns 
alter table DME_ALGORITHM_META add required inteGER default 1;
-- Add comments to the columns 
comment on column DME_ALGORITHM_META.required
  is '是否必须，1：必须；0：可选';
  
  -- Add/modify columns 
alter table DME_RULESTEP_HOP add name varchar2(50);
-- Add comments to the columns 
comment on column DME_RULESTEP_HOP.name
  is '连接线的名称';
  
  
  -- 创建注释表
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

-- 添加属性 
alter table DME_RULESTEP_ATTRIBUTE add isneedprecursor inteGER default 0;
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.isneedprecursor
  is '是否需要前驱，表示依赖其它步骤的参数，如果是1，则attribute_value格式：${步骤编码:参数编码}';
  
  -- Add/modify columns 
alter table DME_RULESTEP rename column step_name to NAME;
alter table DME_RULESTEP add remark varchar2(255);
-- Add comments to the columns 
comment on column DME_RULESTEP.remark
  is '步骤备注信息';

-- Add/modify columns 
alter table DME_TASK add nodeserver varchar2(50);
-- Add comments to the columns 
comment on column DME_TASK.nodeserver
  is '节点服务器';

-- 20180715
alter table DME_TASK_RESULT add status varchar2(20);
-- Add comments to the columns 
comment on column DME_TASK_RESULT.status
  is '状态，运行中：running，停止：stop，成功：success，失败：fail';
  
  --20180716 添加数据源类型
  insert into DME_DATASOURCE_TYPE
values(seq_dme_datasource_type.nextval, 'ef2ad4ffbe4d466cb11e0edc97321d75', 'MONGODB', 'mongodb')

insert into DME_RULESTEP_TYPE
values(seq_dme_rulestep_type.nextval, 'MongodbOutput', 'mongo输出', 'mongo输出', '输出');
--插入数据源-mongo
insert into DME_DATASOURCE
values(seq_dme_datasource.nextval, '5655aa2d80e5474cadd7ba9cb7c57d15', '文件数据库', 'MONGODB', '{
      "Connection": "mongodb://192.168.1.67:27017",
      "DataBase": "DME",
      "userName":"",
      "password":""
    }', 1531735840000, '非结构数据存储');
    
    insert into DME_RULESTEP
values(seq_dme_rulestep.nextval, '6a26dff0f14a42a2a07d1f7fc757722f', 21, 234,456, 41, 41, '输出', 'mongo输出');

insert into DME_RULESTEP_HOP 
values(seq_dme_rulestep_hop.nextval, 21, 41, 41, 85, 1, '输出');

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
  is '任务与规则步骤关联关系';
-- Add comments to the columns 
comment on column DME_TASK_RuleSTEP.syscode
  is '系统唯一编码';
comment on column DME_TASK_RuleSTEP.id
  is '主键id';
comment on column DME_TASK_RuleSTEP.task_id
  is '任务id';
comment on column DME_TASK_RuleSTEP.rulestep_id
  is '规则步骤id';
comment on column DME_TASK_RuleSTEP.status
  is '状态，运行中：running，停止：stop，成功：success，失败：fail';
comment on column DME_TASK_RuleSTEP.createtime
  is '创建时间，毫秒';
 comment on column DME_TASK_RuleSTEP.Lasttime
  is '最后修改时间，毫秒';
  
alter table DME_TASK_RuleSTEP add primary key(id);

create sequence SEQ_DME_TASK_RuleSTEP
minvalue 1
maxvalue 9999999999999999999999999
start with 1
increment by 1
cache 20;

-- 添加模型分类
alter table DME_MODEL add category varchar2(50);
-- Add comments to the columns 
comment on column DME_MODEL.category
  is '分类';
  
  -- Add/modify columns 
alter table DME_RULESTEP_ATTRIBUTE add rowindex inteGER default 0;
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.rowindex
  is '行号，默认为0，用于标识多行索引号';
  
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
  is '模型图片';
-- Add comments to the columns 
comment on column DME_MODEL_IMG.id
  is '主键';
comment on column DME_MODEL_IMG.model_id
  is '模型id';
comment on column DME_MODEL_IMG.version_id
  is '版本id';
comment on column DME_MODEL_IMG.imgcode
  is '图片唯一编码';
  comment on column DME_MODEL_IMG.suffix
  is '后缀';
  comment on column DME_MODEL_IMG.SOURCENAME
  is '源文件名';
  comment on column DME_MODEL_IMG.contentType
  is '类型';
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
  is '状态。0：删除；1：正常';

-- Add/modify columns 
alter table DME_MODEL_VERSION add status integer default 1;
-- Add comments to the columns 
comment on column DME_MODEL_VERSION.status
  is '状态。0：删除；1：正常';
  
  
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
  is '模型类型';
-- Add comments to the columns 
comment on column DME_MODEL_TYPE.id
  is '主键ID';
comment on column DME_MODEL_TYPE.syscode
  is '唯一编码';
comment on column DME_MODEL_TYPE.name
  is '名称';
comment on column DME_MODEL_TYPE.createtime
  is '创建时间';
comment on column DME_MODEL_TYPE.lasttime
  is '修改时间';
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
  is '模型类型ID';
alter table DME_MODEL add model_type_code varchar2(38);
-- Add comments to the columns 
comment on column DME_MODEL.model_type_code
  is '模型类型编码';
  
insert into DME_DATASOURCE_TYPE
values
  (seq_dme_datasource_type.nextval,
   '5750049705814c6cb940d0f4c19920cf',
   'DME_FILESYSTEM',
   'dme文件系统，主要存储于分布式文件系统');
  commit;
  
     -- Add/modify columns 
alter table DME_RULESTEP_ATTRIBUTE add attribute_type integer default 0;
-- Add comments to the columns 
comment on column DME_RULESTEP_ATTRIBUTE.attribute_type
  is '属性类别。0：一般属性；1：运行时属性（意味模型运行时需要指定）';
  
  --建立索引
  create index idx_dme_model on dme_model(syscode);
  create index idx_dme_model_type on dme_model_type(syscode);
  create index idx_DME_RULESTEP on DME_RULESTEP(syscode);

  
  



  
