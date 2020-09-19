provider "aws" {
  version = "~> 2.70"
  access_key = "${var.aws_access_key}"
  secret_key = "${var.aws_secret_key}"
  region= "${var.aws_region}"
}

module "database" {
  source = "./modules/rds"
  name = "bank-db-${var.environment}"
  database_name = "${var.db_database}"
  password = "${var.db_password}"
  username = "${var.db_user}"
  port = "${var.db_port}"
}
