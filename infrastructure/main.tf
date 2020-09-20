provider "aws" {
  version = "~> 2.70"
  access_key = "${var.aws_access_key}"
  secret_key = "${var.aws_secret_key}"
  region= "${var.aws_region}"
}

terraform {
  backend "s3" {
    bucket = "bank-app-infrastructure"
    key = "bank-app.tfstate"
    region = "eu-west-1"
  }
}

module "main" {
  source = "./main"
  environment = "${var.environment}"
  db_database = "${var.db_database}"
  db_password = "${var.db_password}"
  db_user = "${var.db_user}"
  db_port = "${var.db_port}"
}
