insert into
  account_owner (owner_id, first_name, middle_name, last_name, nationality, date_of_birth)
values
  (1,   'fname',   'mname',  'lname',  'Austria',  '2000-01-01 00:00:00'),
  (2,   'foo',     'bar',    'baz',    'Sweden',   '1997-01-01 00:00:00');

insert into
  bank_account (account_id, owner_id, account_number, balance_eur_cents)
values
  (1, 1, 'FI8056420320015046', 1000000),
  (2, 2, 'FI8056420320015046', 2000000);
