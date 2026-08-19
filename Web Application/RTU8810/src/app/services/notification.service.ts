import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';


export interface INotification {
  id: number;
  type: string;
  header: string;
  text: string;
  removePreviousNotifications: boolean;
  processed: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private notificationQueue: INotification[];
  private NotificationQueue: Observable<INotification[]>;
  private _NotificationQueue: BehaviorSubject<INotification[]>;
  private counter = 0;

  constructor() {
    this.notificationQueue = [];
    this._NotificationQueue = <BehaviorSubject<INotification[]>>new BehaviorSubject({});
    this.NotificationQueue = this._NotificationQueue.asObservable();
    this._NotificationQueue.next(this.notificationQueue);
  }

  get() {
    return this._NotificationQueue.asObservable();
  }

  processed( id: number ) {
    this.notificationQueue = this.notificationQueue.filter(obj => obj.id !== id);
  }

  error( textMsg: string, header: string = '', removePreviousNotifications: boolean = false  ) {
    const message:  INotification = {
      id: this.counter++,
      type: 'error',
      header: header,
      text: textMsg,
      removePreviousNotifications: removePreviousNotifications,
      processed: false,
    };
    if ( removePreviousNotifications ) {
      this.notificationQueue = [];
    }
    this.notificationQueue.push( message );
    this._NotificationQueue.next(this.notificationQueue);
  }

  success( textMsg: string, header: string = '', removePreviousNotifications: boolean = false ) {
    const message:  INotification = {
      id: this.counter++,
      type: 'success',
      header: header,
      text: textMsg,
      removePreviousNotifications: removePreviousNotifications,
      processed: false,
    };
    if ( removePreviousNotifications ) {
      this.notificationQueue = [];
    }
    this.notificationQueue.push( message );
    this._NotificationQueue.next(this.notificationQueue);
  }

  exception( textMsg: string , header: string = '', removePreviousNotifications: boolean = false) {
    const message:  INotification = {
      id: this.counter++,
      type: 'error',
      header: header,
      text: textMsg,
      removePreviousNotifications: removePreviousNotifications,
      processed: false,
    };
    if ( removePreviousNotifications ) {
      this.notificationQueue = [];
    }
    this.notificationQueue.push( message );
    this._NotificationQueue.next(this.notificationQueue);
  }
}
