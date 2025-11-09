import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-callback',
  imports: [],
  templateUrl: './callback.html',
  styleUrl: './callback.css',
  styles: [
    `
      .callback-container {
        padding: 20px;
        text-align: center;
      }
    `,
  ],
})
export class Callback {
  constructor(private router: Router) {}

  ngOnInit() {
    // The Auth0 library handles the callback automatically
    // You can add additional logic here if needed
    setTimeout(() => {
      this.router.navigate(['/dashboard']); // Redirect after processing
    }, 2000);
  }
}
