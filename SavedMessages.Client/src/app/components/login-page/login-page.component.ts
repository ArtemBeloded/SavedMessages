import { Component } from "@angular/core";
import { UserService } from "../../services/user-service/user.service";
import { LoginUser } from "../../models/login-user.model";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { Router } from "@angular/router";
import { UserData } from "../../models/user-model";

@Component({
    selector: 'app-login-page',
    templateUrl: './login-page.component.html',
    styleUrls: ['./login-page.component.scss'],
    standalone: true,
    imports: [FormsModule, CommonModule]
})
export class LoginComponent{
    public loginUser: LoginUser = new LoginUser();
    
    constructor(
        private userService: UserService,
        private router: Router){}

    public onLogin() {
        if (this.loginUser.email && this.loginUser.password) {
            this.userService.loginUser(this.loginUser).subscribe((cred: any) => {
                this.userService.getUser(this.loginUser.email).subscribe((res: any) =>{
                    this.userService.removeCredential();
                    this.userService.authenticate(cred.token);
                    this.userService.setUserData(new UserData(res.id, this.loginUser.email, res.firstName, res.lastName));
                    this.userService.userLogin();
                    this.router.navigateByUrl('');
                });
            });
        }
    }

    public goToRegistration() {
        this.router.navigateByUrl('/Registration');
    }
}