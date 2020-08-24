\c bank_db

create table account_owner(
  "owner_id" bigserial primary key,
  "first_name" varchar(255) not null,
  "middle_name" varchar(255),
  "last_name" varchar(255) not null,
  "nationality" varchar(255) not null,
  "date_of_birth" date not null
);

create table bank_account(
  "account_id" bigserial primary key,
  "owner_id" bigint references account_owner (owner_id),
  "account_number" varchar(255) not null,
  "balance_eur_cents" bigint not null default 0
);
