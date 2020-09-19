insert into
  account_owner (first_name, middle_name, last_name, nationality, date_of_birth)
values
  ('fname', 'mname', 'lname', 'Austria', '2000-01-01 00:00:00');

insert into
  bank_account (owner_id, account_number, balance_eur_cents)
values
  (1, 'FI8056420320015046', 1000000),
  (2, 'FI8056420320015046', 2000000);
