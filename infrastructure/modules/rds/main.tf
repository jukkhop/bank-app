data "aws_vpc" "default" {
  default = true
}

data "aws_subnet_ids" "all" {
  vpc_id = "${data.aws_vpc.default.id}"
}

resource "aws_db_subnet_group" "example" {
  name       = "${var.name}"
  subnet_ids = ["${data.aws_subnet_ids.all.ids}"]

  tags = {
    Name = "${var.name}"
  }
}

resource "aws_db_parameter_group" "example" {
  name   = "${var.name}"
  family = "${var.family}"

  tags = {
    Name = "${var.name}"
  }
}

resource "aws_security_group" "db_instance" {
  name   = "${var.name}"
  vpc_id = "${data.aws_vpc.default.id}"
}

resource "aws_security_group_rule" "allow_db_access" {
  cidr_blocks       = ["0.0.0.0/0"]
  from_port         = "${var.port}"
  protocol          = "tcp"
  security_group_id = "${aws_security_group.db_instance.id}"
  to_port           = "${var.port}"
  type              = "ingress"
}

resource "aws_db_instance" "example" {
  allocated_storage      = "${var.allocated_storage}"
  db_subnet_group_name   = "${aws_db_subnet_group.example.id}"
  engine                 = "${var.engine_name}"
  engine_version         = "${var.engine_version}"
  identifier             = "${var.name}"
  instance_class         = "${var.instance_class}"
  name                   = "${var.database_name}"
  parameter_group_name   = "${aws_db_parameter_group.example.id}"
  password               = "${var.password}"
  port                   = "${var.port}"
  publicly_accessible    = true
  skip_final_snapshot    = true
  username               = "${var.username}"
  vpc_security_group_ids = ["${aws_security_group.db_instance.id}"]

  tags = {
    Name = "${var.name}"
  }
}
