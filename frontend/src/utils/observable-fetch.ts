import { Observable, Observer, of } from 'rxjs'
import { fromFetch } from 'rxjs/fetch'
import { catchError, startWith, switchMap } from 'rxjs/operators'

function observableFromFetch<T>(url: URL, init?: RequestInit): Observable<T> {
  return fromFetch(url.toString(), init).pipe(
    switchMap((response: Response) => {
      if (response.ok) {
        return response.json()
      }
      return of({
        loading: false,
        error: true,
        message: response.statusText,
      })
    }),
    startWith({ loading: true, error: false }),
    catchError((err: Error) => {
      return of({ loading: false, error: true, message: err.message })
    }),
  ) as Observable<T>
}

function observeFetch<T>(url: URL, init: RequestInit, observer: Observer<T>): void {
  observableFromFetch<T>(url, init).subscribe(
    (value) => observer.next(value),
    (err) => observer.error(err),
  )
}

export { observeFetch }
