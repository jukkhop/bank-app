module "rds" {
  source = "../rds"

  name     = "bank-app-${var.environment}-db"
  database = var.db_database
  password = var.db_password
  username = var.db_user
  port     = var.db_port
}

module "frontend" {
  source = "../frontend"

  certificate_arn = var.frontend_certificate_arn
  dns_name        = var.frontend_dns_name
  dns_zone_id     = var.frontend_dns_zone_id
}
