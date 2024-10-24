import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../enviroment/enviroment";
import { LoginUser } from "../../models/login-user.model";
import { RegistrationUser } from "../../models/registration-user.model";
import { Observable, Subject } from "rxjs";
import { UserData } from "../../models/user-model";

@Injectable({
    providedIn: 'root'
})
export class UserService{
    private httpClient = inject(HttpClient);
    private apiURL: string;
    private subject$ = new Subject<any>();

    constructor(){
        this.apiURL = environment.apiURL;
    }

    public loginUser(loginUser: LoginUser){
        return this.httpClient.post(this.apiURL + '/api/users/login', loginUser);
    }

    public isAuthenticate(): boolean {
        return localStorage.getItem('userToken') != null 
        ? true 
        : false;
    }

    public removeCredential(){
        if(this.isAuthenticate()){
            localStorage.removeItem('userToken');
            localStorage.removeItem('userData');
        }
    }

    public authenticate(credential: string){
        if(!this.isAuthenticate()){
            localStorage.setItem('userToken', credential);
        }
    }

    public registrationUser(registrationUser: RegistrationUser){
        return this.httpClient.post(this.apiURL + '/api/users/registration', registrationUser);
    }

    public getUser(email: string){
        let params = new HttpParams();
        params = params.append('email', email);
        return this.httpClient.get(this.apiURL + '/api/users/', {params: params});
    }

    public userAuth(): Observable<any> {
        return this.subject$.asObservable();
    }

    public userLogin() {
        this.subject$.next({});
    }

    public setUserData(userData: UserData){
        localStorage.setItem('userData', JSON.stringify(userData));
    }

    public getUserData(){
        return JSON.parse(localStorage.getItem('userData')!);
    }
}