/*
Navicat MySQL Data Transfer

Source Server         : wos
Source Server Version : 50624
Source Host           : 192.168.139.131:3306
Source Database       : woscache

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2016-09-28 22:06:12
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for titlematch
-- ----------------------------
DROP TABLE IF EXISTS `titlematch`;
CREATE TABLE `titlematch` (
  `title` varchar(255) NOT NULL COMMENT '检索的标题',
  `UT` varchar(50) NOT NULL COMMENT '匹配到的标题的馆藏号',
  `titleMatchValue` double NOT NULL COMMENT '标题匹配匹配度',
  `yearMatch` tinyint(4) NOT NULL,
  PRIMARY KEY (`title`,`UT`,`titleMatchValue`,`yearMatch`),
  KEY `wosUT` (`UT`),
  CONSTRAINT `wosUT` FOREIGN KEY (`UT`) REFERENCES `wosdata` (`UT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for wosdata
-- ----------------------------
DROP TABLE IF EXISTS `wosdata`;
CREATE TABLE `wosdata` (
  `PT` tinytext COMMENT '出版物类型',
  `AU` tinytext COMMENT '作者',
  `BA` varchar(255) DEFAULT NULL,
  `BE` varchar(255) DEFAULT NULL,
  `GP` varchar(255) DEFAULT NULL,
  `AF` tinytext,
  `BF` varchar(255) DEFAULT NULL,
  `CA` varchar(255) DEFAULT NULL,
  `TI` tinytext COMMENT '标题',
  `SO` varchar(255) DEFAULT NULL COMMENT '来源出版物',
  `SE` varchar(255) DEFAULT NULL,
  `BS` varchar(255) DEFAULT NULL,
  `LA` varchar(255) DEFAULT NULL COMMENT '语种',
  `DT` varchar(255) DEFAULT NULL COMMENT '文献类型',
  `CT` varchar(255) DEFAULT NULL,
  `CY` varchar(255) DEFAULT NULL,
  `CL` varchar(255) DEFAULT NULL,
  `SP` varchar(255) DEFAULT NULL,
  `HO` varchar(255) DEFAULT NULL,
  `DE` varchar(255) DEFAULT NULL COMMENT '关键词',
  `ID` varchar(255) DEFAULT NULL COMMENT '扩展关键字',
  `AB` text COMMENT '摘要',
  `C1` tinytext COMMENT '作者地址',
  `RP` tinytext COMMENT '通讯作者地址',
  `EM` varchar(255) DEFAULT NULL COMMENT '电子邮件地址',
  `RI` varchar(255) DEFAULT NULL,
  `OI` varchar(255) DEFAULT NULL,
  `FU` varchar(255) DEFAULT NULL COMMENT '基金资助机构和授权号',
  `FX` tinytext COMMENT '基金资助正文',
  `CR` tinytext COMMENT '引用的参考文献',
  `NR` varchar(255) DEFAULT NULL COMMENT '引用的参考文献数',
  `TC` varchar(255) DEFAULT NULL COMMENT '被引频次计数(WoS核心合集)',
  `Z9` varchar(255) DEFAULT NULL COMMENT '被引频次合计（WoS核心、BCI 和 CSCD）',
  `U1` varchar(255) DEFAULT NULL,
  `U2` varchar(255) DEFAULT NULL,
  `PU` varchar(255) DEFAULT NULL COMMENT '出版商',
  `PI` varchar(255) DEFAULT NULL COMMENT '出版商所在城市',
  `PA` varchar(255) DEFAULT NULL COMMENT '出版商地址',
  `SN` varchar(255) DEFAULT NULL COMMENT '国际标准期刊号 (ISSN)',
  `EI` varchar(255) DEFAULT NULL,
  `BN` varchar(255) DEFAULT NULL,
  `J9` varchar(255) DEFAULT NULL COMMENT '来源文献名称缩写',
  `JI` varchar(255) DEFAULT NULL COMMENT 'ISO 来源文献名称缩写',
  `PD` varchar(255) DEFAULT NULL COMMENT '出版日期',
  `PY` varchar(255) DEFAULT NULL COMMENT '出版年',
  `VL` varchar(255) DEFAULT NULL COMMENT '卷',
  `IS` varchar(255) DEFAULT NULL COMMENT '期',
  `PN` varchar(255) DEFAULT NULL,
  `SU` varchar(255) DEFAULT NULL,
  `SI` varchar(255) DEFAULT NULL,
  `MA` varchar(255) DEFAULT NULL,
  `BP` varchar(255) DEFAULT NULL COMMENT '开始页',
  `EP` varchar(255) DEFAULT NULL COMMENT '结束页',
  `AR` varchar(255) DEFAULT NULL,
  `DI` varchar(255) DEFAULT NULL COMMENT '数字对象标识符 (DOI)',
  `D2` varchar(255) DEFAULT NULL,
  `PG` varchar(255) DEFAULT NULL COMMENT '页数',
  `WC` varchar(255) DEFAULT NULL COMMENT 'Web of Science 类别',
  `SC` varchar(255) DEFAULT NULL COMMENT '研究方向',
  `GA` varchar(255) DEFAULT NULL COMMENT '文献传递号',
  `UT` varchar(255) NOT NULL COMMENT '入藏号',
  `PM` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`UT`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
