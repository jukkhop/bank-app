terraform {
  backend "s3" {
    bucket = "bank-app-infrastructure"
    key    = "bank-app.tfstate"
    region = "eu-west-1"
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 2.70"
    }
  }

  required_version = ">= 0.13, < 0.14"
}

provider "aws" {
  access_key = var.aws_access_key
  secret_key = var.aws_secret_key
  region     = var.aws_region
}

module "main" {
  source = "./modules/main"

  db_database              = var.db_database
  db_password              = var.db_password
  db_port                  = var.db_port
  db_user                  = var.db_user
  environment              = var.environment
  frontend_certificate_arn = var.frontend_certificate_arn
  frontend_dns_name        = var.frontend_dns_name
  frontend_dns_zone_id     = var.frontend_dns_zone_id
}
