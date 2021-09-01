import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  users: any;
  constructor(private http: HttpClient) {
  }
  ngOnInit() {
    this.getUsers();
  }
  getUsers() {
    this.http.get('https://localhost:5001/api/Users')
      .subscribe(resopnse => { this.users = resopnse ;console.log(resopnse)},
        error => { console.log(error) });
  }
}
