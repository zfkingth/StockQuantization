﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Stock.WebAPI.Migrations
{
    public partial class changeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"ALTER TABLE `stockquantization`.`priceset` CHANGE COLUMN `Unit` `Unit` TINYINT(1) UNSIGNED NOT NULL ,CHANGE COLUMN `Code` `Code` VARCHAR(15) CHARACTER SET 'latin1' NOT NULL ;");
            migrationBuilder.Sql(@"ALTER TABLE `stockquantization`.`tempprice` CHANGE COLUMN `Unit` `Unit` TINYINT(1) UNSIGNED NOT NULL ,CHANGE COLUMN `Code` `Code` VARCHAR(15) CHARACTER SET 'latin1' NOT NULL ;");
            migrationBuilder.Sql(@"ALTER TABLE `stockquantization`.`securitiesset` CHANGE COLUMN `Type` `Type` TINYINT(1) UNSIGNED NOT NULL,CHANGE COLUMN `Code` `Code` VARCHAR(15) CHARACTER SET 'latin1' NOT NULL ;");
            migrationBuilder.Sql(@"ALTER TABLE `stockquantization`.`stockxrxd` CHANGE COLUMN `Code` `Code` VARCHAR(15) CHARACTER SET 'latin1' NOT NULL ;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}