# SE-Senior-Project
Software Engineering Senior Project at Oregon Tech

Epics and User Stories for Easily Buy And Sell Web app.

Epic 1: Anonymous user/Registered User/Admin browsing the website.
 	Stories:
As a user, I should be able to go to the website and see the homepage with certain ads and options to login or register or search ads so that I can view the homepage.
As a user, I should be able to search for ads by typing the ads name or tags so that I can see all the ads and their basic details.
As a user, when I click on an ad image or title, it should take me to the Ad page so that I can view the details of the ad.
As a user, I should be able to slide the images by clicking and dragging the mouse so that I can see more Images of the Ad if available. 
As a user, I should be able to see the product information so that I can read the details about the product.
As a user, I should be able to click on the “Creator” tab so that I can see the details of the creator of the ad.
As a user, I should be able to click on the “Make a Deal” button so that a form modal pops up and I can fill up a form to send a deal to the ad creator.
As a user, I should be able to click on ad tags which will show me the ads with that tag.

Epic 2: As a user, I want to register for an account so that I can login and manage my account.
	Stories: Registering an Account
As a user, I should be able to go to the homepage, so that I can see the Register button.
As a user, I should be able to see the register button and click on it so that I can see the register form and fill the form to register an account.
As a user, I should be able to fill up the form as required and click on “Register” button so that I can register an account. 
As a user, I should get error notification with message near an input if I don’t fill up the form as required so that I can figure out what the problem is and refill the form again properly. 
As a user, I should get notification that my account is registered so that I can confirm that it is registered.
As a user, I should get an email from the web app to confirm the email so that I can confirm the email of the account I registered.
As a user, I should be able to click on the link in the email to confirm the account.
As a user, I should see a notification in the web app that the email is confirmed so that I can log into the account.

	Stories: Logging Into account
As a user, I should be able to click on login button in the homepage so that I can see the drop down login form and enter the credentials to log in.
As a user, I should get a notification if the credentials are not correct so that I can refill the form and log in.

	Stories: Log Off from the account
As a user, I should be able to click on my username and see the drop down menu so that I can see the log off button.
As a user, I should be able to click on the log off button so that I log off from the web app.

	Stories: Forgot Password
As a user, I should be able to see forgot password button in the login form so that I can click on it to goto the forgot password page.
As a user, I should be able to enter my email in the Forgot password input and click on “Send Me” so that the web app sends me instructions to reset the password.
As a user, I should get error message if the email is not correct. 
As a user, I should get an email with a link after submitting the Forgot Password form so that I can click on the reset button to reset the password.
As a user, I should be able to click on the link in the Forgot Password email so that it opens a page in the browser where I can enter new password so that I can change to a new password.
As a user, I should get error notification near the input box if the password requirement is not fulfilled.

	Stories: Manage profile/Account
As a registered user, I want to be able to click on my user name so that I can see the drop down menu.
As a  registered user, I want to be able to see “Manage Profile” in the drop down menu so that I can go to the manage profile page so that I can edit my profile. 
As a registered  user, I want to be able to add an image or remove the current image in the manage profile page so that I can add or change the current profile picture.
As a registered  user, I want to be able to see an edit button in the manage profile page so that I change certain profile data.
As a registered  user, I want to be able to see Followers and Following in the manage profile page so that when I click on the numbers beside it, I can see who my followers are or who I am following so that I can either keep following or unfollow them.
	
	Stories: Change Password
As a  registered user, I should be able to get to the Manage profile page so that I can see “Change password” button in dashboard.
As a  registered user, I should be able to click on the “Change Password“ button which will take me to the change password form. 
As a  registered user, I should be able to enter my current password and new password in the Change password form and click on “Change” so that I can change the password.
As a registered  user, I should get the error message if the password requirement fails so that I know what should be updated in the form to make it work.

Epic 3: As a registered user, I want to be able to post and manage ad so that others can see the ad and contact me if they want to.
	Stories: Post an Ad
As a registered user, I want to be able to click on “Post an Ad” button so that I can see the post ad form and fill it up.
As a registered user, I want to be able to delay the start date of the ad while filling up the form so that It doesn’t show until the “Start On” date.
As a registered user, I want to be able to make some font attribute changes in the description section of the post ad form so that it appears nicely when the ad is posted.
As a registered user, I want to be able to add three images to the ad with other ad details and click on the “Create Ad” so that I can post the ad. 
As a registered user, I want to be able to get error messages if the form is not correctly filled so that I know what error occurred.
	Stories: Edit an Ad
As a registered user, I want to be able to click on my username after logging in so that I see the “Manage Ads” drop down menu and click on it so that I can get to the Manage Ads page.
As a registered user, I want to be able to see all my ads so that I can delete, edit, renew, or pause my ad. 
As a registered user, I want to be able to hover on the buttons and see a pop up that shows what the button is for so that I know what I am clicking on/for.
As a registered user, I want to be able to see the status of the ad so that I know if the ad is live or is in other status.
As a registered user, I want to be able to click on the ad title so that I can view the full ad.
As an ad owner, I want to be able to click on the delete button to get a delete confirmation notification so that I can rethink before actually deleting the ad.
As an ad owner, I want to be able to click on the confirm button so that I can delete the ad.
As an ad owner, I want to be able to click on the edit button of the ad so that I can see the edit ad page with ad details. 
As an ad owner, I want to be able to change most portion of the ad and click on “Update” button to update the changes. 
As an ad owner, I want to be able to see and click on the cancel button so that I can cancel the changes.
As an ad owner, I want to be able to click on the renew button so that I can renew the ad for next 30 days.
As an ad owner, I want to be able to click on the “Pause” button to pause the ad so that the ad changes it’s status to paused and doesn’t show up for other users.

Epic 4: As an Admin, I should be able to manage categories and any ad.
	Stories: Manage and Post Categories
As an Admin, I want to be able to click on my username and then Admin tools in the drop down menu so that I can goto the Admin page.
As an Admin, I want to be able to see the Categories page with a dashboard with different options so that I can see the list of current categories and  manage it. 
As an Admin, when I hover over the buttons it should show me a popover saying what the buttons are for so that I know what I am clicking onto.
As an Admin, I want to be able to click on the Sub Categories and see all the sub categories for the Category.
As an Admin, I want to be able to click on the delete category button so that I see the confirm delete category notification. 
As an Admin, I want to be able to click on cancel or confirm after I click on the delete category button for the first time so that I can cancel the action of delete if I want to.
As an Admin, I want to be able to click on the edit Category button so that it shows me the details for the Category so that I can see the current category and subcategories for the category.
As an Admin, I want to be able to edit the category name, add or delete any subcategories so that I can change the structure of the Category.
As an Admin, I want to be able to click on “Create Category” button on the dashboard so that I can click it to go to the create category page.
As an Admin, I want to be able to fill up the category form and, sub-categories to it and click on “Create” button so that create a category with subcategories.
As an Admin, I want to be able to get a success message/notification if the category is successfully created so that I know it was created.
As an Admin, I want to be able to see the created category in the All Categories page so that I can confirm that it was created and delete or manage the category.


	Stories: Search/Manage Ads.
As an Admin, I want to be able to click on my username and on see Admin tools option in the drop down so that I can click on it.
As an Admin, I want to be able to click on the Admin tools option in the drop down so that I can go to the admin’s page.
As an Admin, I want to be able to see the Ad button on the dashboard so that I can click on it.
As an Admin, I want to be able to click on the Ad button on the dashboard so that I can see the “Search an Ad” button slide down.
As an Admin, I want to be able to click on the “Search an Ad” button so that I get to the Search Ad page.
As an Admin, I want to be able to type ad id on the search input and click on the search button so that I can search the ad.
As an Admin, I want to be able to see the status of the ad so that I know what status is the ad in.
As an Admin, I want to be able to see if the ad is in the featured section so that I know it is featured ad and will be shown in the slider in the homepage.
As an Admin, I want to be able to click on the Ad title so that a new ad page opens and I can see the ad’s details.
As an Admin, I want to be able to hover over the buttons and the popover with button title pops up so that I know what the buttons are for.
As an Admin, I want to be able to click on the delete ad button so that a confirm option shows up.
As an Admin, I want to be able to click on the cancel or confirm button to confirm delete the ad.
As an Admin, I want to be able to click on renew button so that the ad can be renewed for next 30 days.
As an Admin, I want to be able to add or remove an ad from featured section by toggle clicking the featured button.
As an Admin, I want to be able to click on the pause ad button so that I can change the status of the ad and so that other users cannot view it except the owner.

Epic 5: Other Stories
	Stories: Comments
As a registered user, I want to be able to see a comments section at the bottom of any full ad page so that I can see others comments or post my comment.
As a registered user, I want to be able to click on the post ad text area which will show the submit and cancel button so that I can click on either of them.
As a registered user, I want to click on the Submit button after typing the comment so that the comments get posted.
As a registered user, I want to see my comments on the comments list after I submit my comment so that I can confirm the comment is posted.
As a registered user, I want to be able to delete my comment that I posted by clicking on the “X” button on the top right corner so that I can delete my ad.
As an Admin or the ad creator, I want to be able to delete any comment in the ad page.

	Stories: Follow/Unfollow
		Case 1: From the Ad. 
As a registered user, I want to click on an ad created by other person so that I can see the ad details.
As a registered user, I want to click on the Creator tab so that I can view the Creator’s basic profile.
As a registered user, I want to be able to see the follow button so that I can click on it.
As a registered user, I want to be able to click on the follow button so that I can follow the person and get email notification when the person posts new ad.
As a registered user, I want to be able to see unfollow button if i am already following that person so that I can click on it.
As a registered user, I want to be able to click on the unfollow button so that I can unfollow the user and not get any emails regarding the as posting from that user.
Case 2: From the User profile.
As a registered user, I want to be able to click on my username and see the Manage profile button so that I can click on it.
As a registered user, I want to be able to click on the Manage profile button so that I can goto the manage profile page.
As a registered user, I want to see the number of followers and number of following on the manage profile page so that I can click on it.
As a registered user, I want to click on the Followers number so that a modal pops up and I can see who are following me.
As a registered user, I want to see a button in the near the followers so that I can click on it.
As a registered user, I want to be able to hover over the button so that I know what I am clicking the button for.
As a registered user, I want to be able to click the button so that I can either follow or unfollow the user.
As a registered user, I want to be able to click on the “Following” number so that a modal appears showing who I am following.
As a registered user, I want to be able to see the users I am following and a button next to it so that I can click on it.
As a  registered user, I want to click on the button so that I can unfollow the user.
