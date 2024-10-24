import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RegistrationUser } from '../../models/registration-user.model';
import { UserService } from '../../services/user-service/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration-page',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './registration-page.component.html',
  styleUrls: ['./registration-page.component.scss']
})
export class RegistrationComponent {
    public registrationUser: RegistrationUser = new RegistrationUser()

    constructor(
      private userService: UserService,
      private router: Router){}

    public onRegistration(){
      this.userService.registrationUser(this.registrationUser).subscribe((res: any)=>{
        this.goToLogin();
      });
    }

    public goToLogin(){
      this.router.navigateByUrl('/Login');
    }
}
