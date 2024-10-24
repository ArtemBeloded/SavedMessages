import { HttpInterceptorFn } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { UserService } from '../services/user-service/user.service';
import { Router } from '@angular/router';

export const errorInterceptorFn: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);
  const userService = inject(UserService);
  const router = inject(Router);
  
  return next(req).pipe(
    catchError((error) => {
      if(error.status === 401){
        userService.removeCredential();
        router.navigateByUrl('/Login');
      }
      else if (error.status >= 400 && error.status < 500) {
        const problemDetails = error.error;
        toastr.warning(problemDetails.errors?.[0]?.description, `${problemDetails.errors?.[0]?.code}`);
      } else if (error.status >= 500) {
        toastr.error('An unexpected error occurred', `Error ${error.status}`);
      }
      return throwError(() => error);
    })
  );
};
