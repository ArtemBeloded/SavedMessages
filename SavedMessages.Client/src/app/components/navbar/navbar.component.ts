import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router, RouterOutlet } from "@angular/router";
import { UserService } from "../../services/user-service/user.service";
import { CommonModule } from "@angular/common";
import { Subscription } from "rxjs";

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [RouterOutlet, CommonModule],
    templateUrl: './navbar.component.html',
    styleUrl:'./navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy{
   
    public isAuthenticated: boolean = false;
    public subscription: Subscription;
    
    constructor(
        private userService: UserService,
        private router: Router){
            this.subscription = this.userService.userAuth().subscribe(()=>{
                this.isAuth();
            });
    }
    ngOnDestroy(): void {
        if(this.subscription){
            this.subscription.unsubscribe();
        }
    }

    ngOnInit(): void {
        this.isAuth();
    }

    public isAuth(){
        this.isAuthenticated = this.userService.isAuthenticate();
    }

    public signOut(){
        this.userService.removeCredential();
        this.isAuth();
        this.router.navigateByUrl('/Login');
    }

    public goToLogin(){
        this.router.navigateByUrl('/Login');
    }

    public goToRegistration(){
        this.router.navigateByUrl('/Registration');
    }

    public navigateToMainPage(){
        if(this.isAuthenticated){
            this.router.navigateByUrl('');
        }
        else{
            this.router.navigateByUrl('/Login');
        }
    }
}