insert into
  account_owner (owner_id, first_name, middle_name, last_name, nationality, date_of_birth)
values
  (1,   'Tobias',   'David',  'Gruber',  'Austria',  '1977-06-07 00:00:00'),
  (2,   'Alfred',     'Noah',    'Nielsen',    'Denmark',   '1990-04-08 00:00:00'),
  (3,   'Lucas',     'Levi',    'Jansen',    'Netherlands',   '1985-07-13 00:00:00'),
  (4,   'Lars',     'Mikael',    'Andersson',    'Sweden',   '1996-11-05 00:00:00');

insert into
  bank_account (account_id, owner_id, account_number, balance_eur_cents)
values
  (1, 1, 'AT580946173958687114', 147305),
  (2, 2, 'DK3430822312770917', 266246),
  (3, 3, 'NL44INJJ5714570162', 813236),
  (4, 4, 'SE2036018277173924518942', 128475);
