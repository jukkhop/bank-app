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
  "owner_id" bigint not null references account_owner (owner_id),
  "account_number" varchar(255) not null,
  "balance_eur_cents" bigint not null default 0
);

create table bank_transfer(
  "transfer_id" bigserial primary key,
  "created_at" timestamp with time zone not null,
  "from_account_id" bigint not null references bank_account (account_id),
  "to_account_id" bigint not null references bank_account (account_id),
  "amount_eur_cents" bigint not null,
  "result" varchar(255) not null
);
