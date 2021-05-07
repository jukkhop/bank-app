variable "database" {}
variable "name" {}
variable "password" {}
variable "port" {}
variable "username" {}

variable "allocated_storage" {
  default = 5
}

variable "engine_name" {
  default = "postgres"
}

variable "engine_version" {
  default = "12.5"
}

variable "family" {
  default = "postgres12"
}

variable "instance_class" {
  default = "db.t2.micro"
}
