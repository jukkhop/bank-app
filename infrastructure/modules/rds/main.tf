data "aws_vpc" "default" {
  default = true
}

data "aws_subnet_ids" "all" {
  vpc_id = data.aws_vpc.default.id
}

resource "aws_db_subnet_group" "main" {
  name       = "${var.name}-subnet-group"
  subnet_ids = data.aws_subnet_ids.all.ids

  tags = {
    Name = "${var.name}-subnet-group"
  }
}

resource "aws_db_parameter_group" "main" {
  name   = "${var.name}-parameter-group"
  family = var.family

  tags = {
    Name = "${var.name}-parameter-group"
  }
}

resource "aws_security_group" "main" {
  name   = "${var.name}-security-group"
  vpc_id = data.aws_vpc.default.id

  tags = {
    Name = "${var.name}-security-group"
  }
}

resource "aws_security_group_rule" "allow_db_access" {
  cidr_blocks       = ["0.0.0.0/0"]
  from_port         = var.port
  protocol          = "tcp"
  security_group_id = aws_security_group.main.id
  to_port           = var.port
  type              = "ingress"
}

resource "aws_db_instance" "main" {
  allocated_storage      = var.allocated_storage
  db_subnet_group_name   = aws_db_subnet_group.main.id
  engine                 = var.engine_name
  engine_version         = var.engine_version
  identifier             = var.name
  instance_class         = var.instance_class
  name                   = var.database
  parameter_group_name   = aws_db_parameter_group.main.id
  password               = var.password
  port                   = var.port
  publicly_accessible    = true
  skip_final_snapshot    = true
  username               = var.username
  vpc_security_group_ids = [aws_security_group.main.id]

  tags = {
    Name = var.name
  }
}
