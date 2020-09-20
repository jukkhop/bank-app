module "database" {
  source = "../modules/rds"
  name = "bank-db-${var.environment}"
  database_name = "${var.db_database}"
  password = "${var.db_password}"
  username = "${var.db_user}"
  port = "${var.db_port}"
}
