/*
 Navicat Premium Data Transfer

 Source Server         : api.hengda.show
 Source Server Type    : MySQL
 Source Server Version : 50731
 Source Host           : 192.168.1.250:3306
 Source Schema         : test_HDREAMS

 Target Server Type    : MySQL
 Target Server Version : 50731
 File Encoding         : 65001

 Date: 30/04/2021 18:28:04
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for IdentityServerApiResourceClaims
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiResourceClaims`;
CREATE TABLE `IdentityServerApiResourceClaims`  (
  `Type` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ApiResourceId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ApiResourceId`, `Type`) USING BTREE,
  CONSTRAINT `FK_IdentityServerApiResourceClaims_IdentityServerApiResources_A~` FOREIGN KEY (`ApiResourceId`) REFERENCES `IdentityServerApiResources` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiResourceClaims
-- ----------------------------
INSERT INTO `IdentityServerApiResourceClaims` VALUES ('email', '39fc338b-10e5-81e3-4bbf-84f20a8142c0');
INSERT INTO `IdentityServerApiResourceClaims` VALUES ('email_verified', '39fc338b-10e5-81e3-4bbf-84f20a8142c0');
INSERT INTO `IdentityServerApiResourceClaims` VALUES ('name', '39fc338b-10e5-81e3-4bbf-84f20a8142c0');
INSERT INTO `IdentityServerApiResourceClaims` VALUES ('phone_number', '39fc338b-10e5-81e3-4bbf-84f20a8142c0');
INSERT INTO `IdentityServerApiResourceClaims` VALUES ('phone_number_verified', '39fc338b-10e5-81e3-4bbf-84f20a8142c0');
INSERT INTO `IdentityServerApiResourceClaims` VALUES ('role', '39fc338b-10e5-81e3-4bbf-84f20a8142c0');

-- ----------------------------
-- Table structure for IdentityServerApiResourceProperties
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiResourceProperties`;
CREATE TABLE `IdentityServerApiResourceProperties`  (
  `ApiResourceId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Key` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ApiResourceId`, `Key`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerApiResourceProperties_IdentityServerApiResourc~` FOREIGN KEY (`ApiResourceId`) REFERENCES `IdentityServerApiResources` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiResourceProperties
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerApiResources
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiResources`;
CREATE TABLE `IdentityServerApiResources`  (
  `Id` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `DisplayName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Enabled` tinyint(1) NOT NULL,
  `AllowedAccessTokenSigningAlgorithms` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `ShowInDiscoveryDocument` tinyint(1) NOT NULL,
  `ExtraProperties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ConcurrencyStamp` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `LastModificationTime` datetime(6) NULL DEFAULT NULL,
  `LastModifierId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `DeleterId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DeletionTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiResources
-- ----------------------------
INSERT INTO `IdentityServerApiResources` VALUES ('39fc338b-10e5-81e3-4bbf-84f20a8142c0', 'REAMS', 'REAMS API', NULL, 1, NULL, 1, '{}', '20abd7323439487ab2dfa2c43487335c', '2021-04-30 14:01:33.710894', NULL, '2021-04-30 14:01:41.215602', NULL, 0, NULL, NULL);

-- ----------------------------
-- Table structure for IdentityServerApiResourceScopes
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiResourceScopes`;
CREATE TABLE `IdentityServerApiResourceScopes`  (
  `ApiResourceId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Scope` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ApiResourceId`, `Scope`) USING BTREE,
  CONSTRAINT `FK_IdentityServerApiResourceScopes_IdentityServerApiResources_A~` FOREIGN KEY (`ApiResourceId`) REFERENCES `IdentityServerApiResources` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiResourceScopes
-- ----------------------------
INSERT INTO `IdentityServerApiResourceScopes` VALUES ('39fc338b-10e5-81e3-4bbf-84f20a8142c0', 'REAMS');

-- ----------------------------
-- Table structure for IdentityServerApiResourceSecrets
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiResourceSecrets`;
CREATE TABLE `IdentityServerApiResourceSecrets`  (
  `Type` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ApiResourceId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Expiration` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`ApiResourceId`, `Type`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerApiResourceSecrets_IdentityServerApiResources_~` FOREIGN KEY (`ApiResourceId`) REFERENCES `IdentityServerApiResources` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiResourceSecrets
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerApiScopeClaims
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiScopeClaims`;
CREATE TABLE `IdentityServerApiScopeClaims`  (
  `Type` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ApiScopeId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ApiScopeId`, `Type`) USING BTREE,
  CONSTRAINT `FK_IdentityServerApiScopeClaims_IdentityServerApiScopes_ApiScop~` FOREIGN KEY (`ApiScopeId`) REFERENCES `IdentityServerApiScopes` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiScopeClaims
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerApiScopeProperties
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiScopeProperties`;
CREATE TABLE `IdentityServerApiScopeProperties`  (
  `ApiScopeId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Key` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ApiScopeId`, `Key`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerApiScopeProperties_IdentityServerApiScopes_Api~` FOREIGN KEY (`ApiScopeId`) REFERENCES `IdentityServerApiScopes` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiScopeProperties
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerApiScopes
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerApiScopes`;
CREATE TABLE `IdentityServerApiScopes`  (
  `Id` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Enabled` tinyint(1) NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `DisplayName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Required` tinyint(1) NOT NULL,
  `Emphasize` tinyint(1) NOT NULL,
  `ShowInDiscoveryDocument` tinyint(1) NOT NULL,
  `ExtraProperties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ConcurrencyStamp` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `LastModificationTime` datetime(6) NULL DEFAULT NULL,
  `LastModifierId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `DeleterId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DeletionTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerApiScopes
-- ----------------------------
INSERT INTO `IdentityServerApiScopes` VALUES ('39fc338b-1197-1b11-4a36-50d82aa26310', 1, 'REAMS', 'REAMS API', NULL, 0, 0, 1, '{}', 'ff460cb800074ae5ac7d792298499f9a', '2021-04-30 14:01:33.876522', NULL, NULL, NULL, 0, NULL, NULL);

-- ----------------------------
-- Table structure for IdentityServerClientClaims
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientClaims`;
CREATE TABLE `IdentityServerClientClaims`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `Type`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientClaims_IdentityServerClients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientClaims
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerClientCorsOrigins
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientCorsOrigins`;
CREATE TABLE `IdentityServerClientCorsOrigins`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Origin` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `Origin`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientCorsOrigins_IdentityServerClients_Client~` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientCorsOrigins
-- ----------------------------
INSERT INTO `IdentityServerClientCorsOrigins` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'https://reams.hengda.show:44300');
INSERT INTO `IdentityServerClientCorsOrigins` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'http://reams.hengda.show:4200');
INSERT INTO `IdentityServerClientCorsOrigins` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'https://reams.hengda.show:44307');
INSERT INTO `IdentityServerClientCorsOrigins` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'https://reams.hengda.show:44314');
INSERT INTO `IdentityServerClientCorsOrigins` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'https://localhost:44360');
INSERT INTO `IdentityServerClientCorsOrigins` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'https://reams.hengda.show:44360');

-- ----------------------------
-- Table structure for IdentityServerClientGrantTypes
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientGrantTypes`;
CREATE TABLE `IdentityServerClientGrantTypes`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `GrantType` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `GrantType`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientGrantTypes_IdentityServerClients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientGrantTypes
-- ----------------------------
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'hybrid');
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'authorization_code');
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'client_credentials');
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'password');
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'authorization_code');
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'hybrid');
INSERT INTO `IdentityServerClientGrantTypes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'authorization_code');

-- ----------------------------
-- Table structure for IdentityServerClientIdPRestrictions
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientIdPRestrictions`;
CREATE TABLE `IdentityServerClientIdPRestrictions`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Provider` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `Provider`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientIdPRestrictions_IdentityServerClients_Cl~` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientIdPRestrictions
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerClientPostLogoutRedirectUris
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientPostLogoutRedirectUris`;
CREATE TABLE `IdentityServerClientPostLogoutRedirectUris`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PostLogoutRedirectUri` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `PostLogoutRedirectUri`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientPostLogoutRedirectUris_IdentityServerCli~` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientPostLogoutRedirectUris
-- ----------------------------
INSERT INTO `IdentityServerClientPostLogoutRedirectUris` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'https://localhost:44360/signout-callback-oidc');
INSERT INTO `IdentityServerClientPostLogoutRedirectUris` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'https://reams.hengda.show:44300/signout-callback-oidc');
INSERT INTO `IdentityServerClientPostLogoutRedirectUris` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'http://reams.hengda.show:4200');
INSERT INTO `IdentityServerClientPostLogoutRedirectUris` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'https://reams.hengda.show:44307/authentication/logout-callback');
INSERT INTO `IdentityServerClientPostLogoutRedirectUris` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'https://reams.hengda.show:44314/signout-callback-oidc');

-- ----------------------------
-- Table structure for IdentityServerClientProperties
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientProperties`;
CREATE TABLE `IdentityServerClientProperties`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Key` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `Key`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientProperties_IdentityServerClients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientProperties
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerClientRedirectUris
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientRedirectUris`;
CREATE TABLE `IdentityServerClientRedirectUris`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RedirectUri` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `RedirectUri`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientRedirectUris_IdentityServerClients_Clien~` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientRedirectUris
-- ----------------------------
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'https://reams.hengda.show:44300/signin-oidc');
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'http://reams.hengda.show:4200');
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'https://reams.hengda.show:44307/authentication/login-callback');
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'https://reams.hengda.show:44314/signin-oidc');
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'https://127.hengda.show:44360/swagger/oauth2-redirect.html');
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'https://localhost:44360/swagger/oauth2-redirect.html');
INSERT INTO `IdentityServerClientRedirectUris` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'https://reams.hengda.show:44360/swagger/oauth2-redirect.html');

-- ----------------------------
-- Table structure for IdentityServerClients
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClients`;
CREATE TABLE `IdentityServerClients`  (
  `Id` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ClientId` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ClientName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `ClientUri` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `LogoUri` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Enabled` tinyint(1) NOT NULL,
  `ProtocolType` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `RequireClientSecret` tinyint(1) NOT NULL,
  `RequireConsent` tinyint(1) NOT NULL,
  `AllowRememberConsent` tinyint(1) NOT NULL,
  `AlwaysIncludeUserClaimsInIdToken` tinyint(1) NOT NULL,
  `RequirePkce` tinyint(1) NOT NULL,
  `AllowPlainTextPkce` tinyint(1) NOT NULL,
  `RequireRequestObject` tinyint(1) NOT NULL,
  `AllowAccessTokensViaBrowser` tinyint(1) NOT NULL,
  `FrontChannelLogoutUri` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `FrontChannelLogoutSessionRequired` tinyint(1) NOT NULL,
  `BackChannelLogoutUri` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `BackChannelLogoutSessionRequired` tinyint(1) NOT NULL,
  `AllowOfflineAccess` tinyint(1) NOT NULL,
  `IdentityTokenLifetime` int(11) NOT NULL,
  `AllowedIdentityTokenSigningAlgorithms` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `AccessTokenLifetime` int(11) NOT NULL,
  `AuthorizationCodeLifetime` int(11) NOT NULL,
  `ConsentLifetime` int(11) NULL DEFAULT NULL,
  `AbsoluteRefreshTokenLifetime` int(11) NOT NULL,
  `SlidingRefreshTokenLifetime` int(11) NOT NULL,
  `RefreshTokenUsage` int(11) NOT NULL,
  `UpdateAccessTokenClaimsOnRefresh` tinyint(1) NOT NULL,
  `RefreshTokenExpiration` int(11) NOT NULL,
  `AccessTokenType` int(11) NOT NULL,
  `EnableLocalLogin` tinyint(1) NOT NULL,
  `IncludeJwtId` tinyint(1) NOT NULL,
  `AlwaysSendClientClaims` tinyint(1) NOT NULL,
  `ClientClaimsPrefix` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `PairWiseSubjectSalt` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `UserSsoLifetime` int(11) NULL DEFAULT NULL,
  `UserCodeType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `DeviceCodeLifetime` int(11) NOT NULL,
  `ExtraProperties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ConcurrencyStamp` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `LastModificationTime` datetime(6) NULL DEFAULT NULL,
  `LastModifierId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `DeleterId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DeletionTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `IX_IdentityServerClients_ClientId`(`ClientId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClients
-- ----------------------------
INSERT INTO `IdentityServerClients` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'REAMS_Web', 'REAMS_Web', 'REAMS_Web', NULL, NULL, 1, 'oidc', 1, 0, 1, 1, 0, 0, 0, 0, 'https://reams.hengda.show:44300/Account/FrontChannelLogout', 1, NULL, 1, 1, 300, NULL, 31536000, 300, NULL, 31536000, 1296000, 1, 0, 1, 0, 1, 0, 0, 'client_', NULL, NULL, NULL, 300, '{}', '074f3a43fe334e7880348e56d717b5b8', '2021-04-30 14:01:34.024620', NULL, '2021-04-30 14:01:41.215636', NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerClients` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'REAMS_App', 'REAMS_App', 'REAMS_App', NULL, NULL, 1, 'oidc', 0, 0, 1, 1, 0, 0, 0, 0, NULL, 1, NULL, 1, 1, 300, NULL, 31536000, 300, NULL, 31536000, 1296000, 1, 0, 1, 0, 1, 0, 0, 'client_', NULL, NULL, NULL, 300, '{}', '44fdf98ff68549e58bdf6cb21d1589c5', '2021-04-30 14:01:34.056130', NULL, '2021-04-30 14:01:41.215656', NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerClients` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'REAMS_Blazor', 'REAMS_Blazor', 'REAMS_Blazor', NULL, NULL, 1, 'oidc', 0, 0, 1, 1, 0, 0, 0, 0, NULL, 1, NULL, 1, 1, 300, NULL, 31536000, 300, NULL, 31536000, 1296000, 1, 0, 1, 0, 1, 0, 0, 'client_', NULL, NULL, NULL, 300, '{}', 'd83c83bba1c14cbda75b269b51c8691e', '2021-04-30 14:01:34.165389', NULL, '2021-04-30 14:01:41.215671', NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerClients` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'REAMS_BlazorServerTiered', 'REAMS_BlazorServerTiered', 'REAMS_BlazorServerTiered', NULL, NULL, 1, 'oidc', 1, 0, 1, 1, 0, 0, 0, 0, 'https://reams.hengda.show:44314/Account/FrontChannelLogout', 1, NULL, 1, 1, 300, NULL, 31536000, 300, NULL, 31536000, 1296000, 1, 0, 1, 0, 1, 0, 0, 'client_', NULL, NULL, NULL, 300, '{}', '28e942c22c6d4a198514e6f2afba82de', '2021-04-30 14:01:34.465410', NULL, '2021-04-30 14:01:41.215690', NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerClients` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'REAMS_Swagger', 'REAMS_Swagger', 'REAMS_Swagger', NULL, NULL, 1, 'oidc', 0, 0, 1, 1, 0, 0, 0, 0, NULL, 1, NULL, 1, 1, 300, NULL, 31536000, 300, NULL, 31536000, 1296000, 1, 0, 1, 0, 1, 0, 0, 'client_', NULL, NULL, NULL, 300, '{}', '8fa86876e7e34cc1b3631fe73742d4ab', '2021-04-30 14:01:34.480246', NULL, '2021-04-30 14:01:41.217341', NULL, 0, NULL, NULL);

-- ----------------------------
-- Table structure for IdentityServerClientScopes
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientScopes`;
CREATE TABLE `IdentityServerClientScopes`  (
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Scope` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`ClientId`, `Scope`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientScopes_IdentityServerClients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientScopes
-- ----------------------------
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'address');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'email');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'openid');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'phone');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'profile');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'REAMS');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1201-3ae1-8714-409524f1d774', 'role');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'address');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'email');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'openid');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'phone');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'profile');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'REAMS');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1256-ce60-5609-9c8db0059a86', 'role');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'address');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'email');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'openid');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'phone');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'profile');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'REAMS');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-12d4-5c2d-1e19-203fb7efc6a2', 'role');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'address');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'email');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'openid');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'phone');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'profile');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'REAMS');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-1400-6cda-a0ce-03e70ceee191', 'role');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'address');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'email');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'openid');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'phone');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'profile');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'REAMS');
INSERT INTO `IdentityServerClientScopes` VALUES ('39fc338b-140f-76c6-1bd1-15cdb4a9d16f', 'role');

-- ----------------------------
-- Table structure for IdentityServerClientSecrets
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerClientSecrets`;
CREATE TABLE `IdentityServerClientSecrets`  (
  `Type` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ClientId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Expiration` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`ClientId`, `Type`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerClientSecrets_IdentityServerClients_ClientId` FOREIGN KEY (`ClientId`) REFERENCES `IdentityServerClients` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerClientSecrets
-- ----------------------------
INSERT INTO `IdentityServerClientSecrets` VALUES ('SharedSecret', 'E5Xd4yMqjP5kjWFKrYgySBju6JVfCzMyFp7n2QmMrME=', '39fc338b-1201-3ae1-8714-409524f1d774', NULL, NULL);
INSERT INTO `IdentityServerClientSecrets` VALUES ('SharedSecret', 'E5Xd4yMqjP5kjWFKrYgySBju6JVfCzMyFp7n2QmMrME=', '39fc338b-1256-ce60-5609-9c8db0059a86', NULL, NULL);
INSERT INTO `IdentityServerClientSecrets` VALUES ('SharedSecret', 'E5Xd4yMqjP5kjWFKrYgySBju6JVfCzMyFp7n2QmMrME=', '39fc338b-1400-6cda-a0ce-03e70ceee191', NULL, NULL);
INSERT INTO `IdentityServerClientSecrets` VALUES ('SharedSecret', 'E5Xd4yMqjP5kjWFKrYgySBju6JVfCzMyFp7n2QmMrME=', '39fc338b-140f-76c6-1bd1-15cdb4a9d16f', NULL, NULL);

-- ----------------------------
-- Table structure for IdentityServerDeviceFlowCodes
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerDeviceFlowCodes`;
CREATE TABLE `IdentityServerDeviceFlowCodes`  (
  `Id` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DeviceCode` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `UserCode` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `SubjectId` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `SessionId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `ClientId` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Expiration` datetime(6) NOT NULL,
  `Data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ExtraProperties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ConcurrencyStamp` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `IX_IdentityServerDeviceFlowCodes_DeviceCode`(`DeviceCode`) USING BTREE,
  INDEX `IX_IdentityServerDeviceFlowCodes_Expiration`(`Expiration`) USING BTREE,
  INDEX `IX_IdentityServerDeviceFlowCodes_UserCode`(`UserCode`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerDeviceFlowCodes
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerIdentityResourceClaims
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerIdentityResourceClaims`;
CREATE TABLE `IdentityServerIdentityResourceClaims`  (
  `Type` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `IdentityResourceId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`IdentityResourceId`, `Type`) USING BTREE,
  CONSTRAINT `FK_IdentityServerIdentityResourceClaims_IdentityServerIdentityR~` FOREIGN KEY (`IdentityResourceId`) REFERENCES `IdentityServerIdentityResources` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerIdentityResourceClaims
-- ----------------------------
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('sub', '39fc338b-1086-b433-cfaa-796cfdcb6bef');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('birthdate', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('family_name', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('gender', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('given_name', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('locale', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('middle_name', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('name', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('nickname', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('picture', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('preferred_username', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('profile', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('updated_at', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('website', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('zoneinfo', '39fc338b-10b7-034e-89ef-8d6f2f33a0de');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('email', '39fc338b-10bd-5bcb-822d-dbecc1ae18a5');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('email_verified', '39fc338b-10bd-5bcb-822d-dbecc1ae18a5');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('address', '39fc338b-10bf-1030-5e2e-d79aab2d4468');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('phone_number', '39fc338b-10c3-c554-eb5a-527e8ede139a');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('phone_number_verified', '39fc338b-10c3-c554-eb5a-527e8ede139a');
INSERT INTO `IdentityServerIdentityResourceClaims` VALUES ('role', '39fc338b-10c6-57f8-88b3-a062e70979a4');

-- ----------------------------
-- Table structure for IdentityServerIdentityResourceProperties
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerIdentityResourceProperties`;
CREATE TABLE `IdentityServerIdentityResourceProperties`  (
  `IdentityResourceId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Key` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Value` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`IdentityResourceId`, `Key`, `Value`) USING BTREE,
  CONSTRAINT `FK_IdentityServerIdentityResourceProperties_IdentityServerIdent~` FOREIGN KEY (`IdentityResourceId`) REFERENCES `IdentityServerIdentityResources` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerIdentityResourceProperties
-- ----------------------------

-- ----------------------------
-- Table structure for IdentityServerIdentityResources
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerIdentityResources`;
CREATE TABLE `IdentityServerIdentityResources`  (
  `Id` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `DisplayName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `Enabled` tinyint(1) NOT NULL,
  `Required` tinyint(1) NOT NULL,
  `Emphasize` tinyint(1) NOT NULL,
  `ShowInDiscoveryDocument` tinyint(1) NOT NULL,
  `ExtraProperties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ConcurrencyStamp` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `CreatorId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `LastModificationTime` datetime(6) NULL DEFAULT NULL,
  `LastModifierId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL DEFAULT 0,
  `DeleterId` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `DeletionTime` datetime(6) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerIdentityResources
-- ----------------------------
INSERT INTO `IdentityServerIdentityResources` VALUES ('39fc338b-1086-b433-cfaa-796cfdcb6bef', 'openid', 'Your user identifier', NULL, 1, 1, 0, 1, '{}', '66bff3c6449c4589ac2dc87f78e381d9', '2021-04-30 14:01:33.708174', NULL, NULL, NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerIdentityResources` VALUES ('39fc338b-10b7-034e-89ef-8d6f2f33a0de', 'profile', 'User profile', 'Your user profile information (first name, last name, etc.)', 1, 0, 1, 1, '{}', 'e0ba6f0628b84e378ccda7a879161ce9', '2021-04-30 14:01:33.708237', NULL, NULL, NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerIdentityResources` VALUES ('39fc338b-10bd-5bcb-822d-dbecc1ae18a5', 'email', 'Your email address', NULL, 1, 0, 1, 1, '{}', '89ad9745498c44c987abba1f34922058', '2021-04-30 14:01:33.708265', NULL, NULL, NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerIdentityResources` VALUES ('39fc338b-10bf-1030-5e2e-d79aab2d4468', 'address', 'Your postal address', NULL, 1, 0, 1, 1, '{}', '9a2730820dda432aac2f6c8bbd7c249c', '2021-04-30 14:01:33.710837', NULL, NULL, NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerIdentityResources` VALUES ('39fc338b-10c3-c554-eb5a-527e8ede139a', 'phone', 'Your phone number', NULL, 1, 0, 1, 1, '{}', 'eb9e7e91140a48f6aa470928737449e7', '2021-04-30 14:01:33.710860', NULL, NULL, NULL, 0, NULL, NULL);
INSERT INTO `IdentityServerIdentityResources` VALUES ('39fc338b-10c6-57f8-88b3-a062e70979a4', 'role', 'Roles of the user', NULL, 1, 0, 0, 1, '{}', '20e7b6ea1d3547a38c721dc8ace07093', '2021-04-30 14:01:33.710868', NULL, NULL, NULL, 0, NULL, NULL);

-- ----------------------------
-- Table structure for IdentityServerPersistedGrants
-- ----------------------------
DROP TABLE IF EXISTS `IdentityServerPersistedGrants`;
CREATE TABLE `IdentityServerPersistedGrants`  (
  `Key` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `SubjectId` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `SessionId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `ClientId` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `CreationTime` datetime(6) NOT NULL,
  `Expiration` datetime(6) NULL DEFAULT NULL,
  `ConsumedTime` datetime(6) NULL DEFAULT NULL,
  `Data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Id` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ExtraProperties` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL,
  `ConcurrencyStamp` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Key`) USING BTREE,
  INDEX `IX_IdentityServerPersistedGrants_Expiration`(`Expiration`) USING BTREE,
  INDEX `IX_IdentityServerPersistedGrants_SubjectId_ClientId_Type`(`SubjectId`, `ClientId`, `Type`) USING BTREE,
  INDEX `IX_IdentityServerPersistedGrants_SubjectId_SessionId_Type`(`SubjectId`, `SessionId`, `Type`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of IdentityServerPersistedGrants
-- ----------------------------

SET FOREIGN_KEY_CHECKS = 1;
