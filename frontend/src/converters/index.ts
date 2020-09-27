import parseDate from 'date-fns/parseISO'
import formatDate from 'date-fns/format'
import { AccountOwner } from '../types'

function fullName(owner: AccountOwner): string {
  return `${owner.firstName} ${owner.lastName}`
}

function showDate(dateString: string): string {
  const parsedDate = parseDate(dateString)
  return formatDate(parsedDate, 'd.M.y HH:mm:ss')
}

function toEuros(cents: number): string {
  const euros = cents / 100
  return `${euros.toFixed(2).replaceAll('.', ',')} €`
}

export { fullName, showDate, toEuros }
