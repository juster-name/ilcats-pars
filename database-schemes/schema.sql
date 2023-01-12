-- MySQL dump 10.13  Distrib 8.0.31, for Linux (x86_64)
--
-- Host: localhost    Database: ilcats_cars_db
-- ------------------------------------------------------
-- Server version	8.0.31-0ubuntu0.22.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Car`
--

DROP TABLE IF EXISTS `Car`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Car` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `market_code` char(2) DEFAULT NULL,
  `market_name` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=260 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Catalog_0`
--

DROP TABLE IF EXISTS `Catalog_0`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Catalog_0` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `complectation_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `complectation_id` (`complectation_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Catalog_1`
--

DROP TABLE IF EXISTS `Catalog_1`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Catalog_1` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `name` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `catalog_0_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `catalog_0_id` (`catalog_0_id`),
  CONSTRAINT `Catalog_1_ibfk_1` FOREIGN KEY (`catalog_0_id`) REFERENCES `Catalog_0` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Complectation`
--

DROP TABLE IF EXISTS `Complectation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Complectation` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `model_id` int DEFAULT NULL,
  `date_prod_range` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `code` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_code_modelId` (`code`,`model_id`),
  KEY `model_id` (`model_id`),
  CONSTRAINT `Complectation_ibfk_1` FOREIGN KEY (`model_id`) REFERENCES `Model` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8262 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ModDataName`
--

DROP TABLE IF EXISTS `ModDataName`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ModDataName` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `name` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=54161 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ModDataPair`
--

DROP TABLE IF EXISTS `ModDataPair`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ModDataPair` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `name_id` int DEFAULT NULL,
  `value_id` int DEFAULT NULL,
  `modification_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_nameId_valueId_modificationId` (`name_id`,`value_id`,`modification_id`),
  KEY `modification_id` (`modification_id`),
  CONSTRAINT `ModDataPair_ibfk_1` FOREIGN KEY (`modification_id`) REFERENCES `Modification` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=54191 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ModDataValue`
--

DROP TABLE IF EXISTS `ModDataValue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ModDataValue` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `value` varchar(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `value` (`value`)
) ENGINE=InnoDB AUTO_INCREMENT=54158 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Model`
--

DROP TABLE IF EXISTS `Model`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Model` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `car_id` int DEFAULT NULL,
  `code` varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `model_name_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_code_carId` (`code`,`car_id`),
  KEY `car_id` (`car_id`),
  KEY `model_name_id` (`model_name_id`),
  CONSTRAINT `Model_ibfk_1` FOREIGN KEY (`car_id`) REFERENCES `Car` (`id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `Model_ibfk_2` FOREIGN KEY (`model_name_id`) REFERENCES `ModelName` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=280 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ModelName`
--

DROP TABLE IF EXISTS `ModelName`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ModelName` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `name` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=133 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Modification`
--

DROP TABLE IF EXISTS `Modification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Modification` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `complectation_id` int DEFAULT NULL,
  `code` varchar(20) DEFAULT NULL,
  `date_prod_range` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_code_complectationId` (`code`,`complectation_id`),
  KEY `complectation_id` (`complectation_id`),
  CONSTRAINT `Modification_ibfk_3` FOREIGN KEY (`complectation_id`) REFERENCES `Complectation` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=54154 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Scheme`
--

DROP TABLE IF EXISTS `Scheme`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Scheme` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
  `code` bigint DEFAULT NULL,
  `count` tinyint DEFAULT NULL,
  `info` varchar(32) DEFAULT NULL,
  `tree_code` varchar(6) DEFAULT NULL,
  `tree` varchar(32) DEFAULT NULL,
  `date_prod_range` varchar(16) DEFAULT NULL,
  `modification_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `modification_id` (`modification_id`),
  CONSTRAINT `Scheme_ibfk_1` FOREIGN KEY (`modification_id`) REFERENCES `Modification` (`id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Temporary view structure for view `modification_view`
--

DROP TABLE IF EXISTS `modification_view`;
/*!50001 DROP VIEW IF EXISTS `modification_view`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `modification_view` AS SELECT 
 1 AS `Code`,
 1 AS `Key`,
 1 AS `Value`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `modification_view`
--

/*!50001 DROP VIEW IF EXISTS `modification_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_unicode_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `modification_view` AS select `Modification`.`code` AS `Code`,`ModDataName`.`name` AS `Key`,`ModDataValue`.`value` AS `Value` from (((`ModDataPair` join `ModDataName` on((`ModDataPair`.`name_id` = `ModDataName`.`id`))) join `ModDataValue` on((`ModDataPair`.`value_id` = `ModDataValue`.`id`))) join `Modification` on((`ModDataPair`.`modification_id` = `Modification`.`id`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-01-12 19:31:20
