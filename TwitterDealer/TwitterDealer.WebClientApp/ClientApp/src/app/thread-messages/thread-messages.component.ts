import { Component, OnInit } from '@angular/core';
import { TweetsService } from '../services/tweets.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { StatusTweet } from '../models/statusTweet';
import { OrgData } from 'angular-org-chart/src/app/modules/org-chart/orgData';

@Component({
  selector: 'app-thread-messages',
  templateUrl: './thread-messages.component.html',
  styleUrls: ['./thread-messages.component.scss']
})
export class ThreadMessagesComponent implements OnInit {
  public link: string;
  public allThread: StatusTweet;
  public showReplies: boolean;
  public showCharts: boolean;
  public orgData: OrgData;

  public userActivityCount: number;
  public likesCount: number;
  public retweetCount: number;
  public userRepliesCount: number;

  constructor(private tweetsService: TweetsService,
              private toastr: ToastrService,
              private router: Router) { }

  ngOnInit(): void {
    this.showReplies = false;
    this.showCharts = false;
  }

  onGetAllTweets(link: string): void{
    this.tweetsService.getFullThread(link).subscribe((res: StatusTweet) => {
      this.allThread = null;
      this.allThread = res;
      this.orgData = this.allThread;
      this.getChartsData(this.allThread);
    }, error => {
      this.toastr.error(error, 'Error ocurred', { positionClass: 'toast-bottom-right' });
    }
    );
  }

  onClear(): void {
    this.link = '';
    this.allThread = null;
    this.orgData = null;
  }

  onShowReplies(): void {
    this.showReplies = !this.showReplies;
  }

  onShowCharts(): void {
    this.showCharts = !this.showCharts;
  }

  private getChartsData(data: StatusTweet): void {
    if (data == null) {
      return;
    }

    this.userActivityCount = data.replies.filter(r => r.tweetText.length > 5).length;
    this.likesCount = data.replies.filter(r => r.favoriteCount >= 1).length;
    this.retweetCount = data.replies.filter(r => r.retweetCount >= 1).length;
    this.userRepliesCount = data.replies.filter(r => r.userName === data.userName).length;
  }
}
