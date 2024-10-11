import { Routes } from '@angular/router';
import { LoginComponent } from './components/login-page/login-page.component';
import { RegistrationComponent } from './components/registration-page/registration-page.component';
import { MessageComponent } from './components/message-page/message-page.component';

export const routes: Routes = [
    {path: '', component: MessageComponent},
    {path: 'Login', component: LoginComponent},
    {path: 'Registration', component: RegistrationComponent}
];
