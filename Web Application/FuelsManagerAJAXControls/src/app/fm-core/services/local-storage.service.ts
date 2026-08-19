import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
/**
 * Wrapper for localStorage on window, possibly can extend or replace
 */
export class LocalStorageService {

  constructor() { }
  /**
   * stores entry locally to the browser regardless of session
   * @param key
   * @param toStore
   */
  store<T>(key: string, toStore: T): void {
      const serilizedToStore = JSON.stringify(toStore);
      window.localStorage.setItem(key, serilizedToStore);
  }

  /**
   * retrieves local key if avaliable, otherwise returns null if empty
   * @param key
   */
  get<T>(key: string): T {
      const serelizedStoredObject = window.localStorage.getItem(key);
      if (serelizedStoredObject == null) {
          return null;
      }
      return <T>(JSON.parse(serelizedStoredObject));
  }

  clear(key: string): void {
      window.localStorage.removeItem(key);
  }

  /**
   * Empties entire cache
   */
  clearAll(): void {
      window.localStorage.clear();
  }
}
