namespace Bank

open Bank.AccountOwnerDb
open Bank.BankAccountDb
open Bank.BankTransferDb
open Bank.Database
open Bank.Config

module Context =

  type IContext =
    abstract Config: Config
    abstract Db: IDatabase
    abstract AccountDb: IBankAccountDb
    abstract OwnerDb: IAccountOwnerDb
    abstract TransferDb: IBankTransferDb

  type Context (config: Config) as this =
    member __.Db = Database (config.Postgres) :> IDatabase
    member __.AccountDb = BankAccountDb (this.Db) :> IBankAccountDb
    member __.OwnerDb = AccountOwnerDb (this.Db) :> IAccountOwnerDb
    member __.TransferDb = BankTransferDb (this.Db) :> IBankTransferDb
