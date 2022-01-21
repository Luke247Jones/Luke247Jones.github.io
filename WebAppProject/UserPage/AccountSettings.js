import React from 'react';
import axios from "axios";
import '../../Components/FormDropdown.css';
import {useState, useEffect} from 'react';
import useFindUser from '../../hooks/useFindUser';
import{ withRouter } from 'react-router-dom';
import ReactGA from "react-ga";

ReactGA.initialize(process.env.GA_TRACKING_CODE);

const postURL = "/users/changePassword";
//const user_id = 1;

const AccountSettings = (props) => {
    const {axiosJWT} = useFindUser();

    useEffect(() => {
        ReactGA.pageview(window.location.pathname + window.location.search);
    });
    /* useState ******************************************************************************************/
    // User Info
    const [firstName, setFirstName] = useState(props.userInfo.firstname);
    const [lastName, setLastName] = useState(props.userInfo.lastname);
    const [email, setEmail] = useState(props.userInfo.email);
    const [streetAddress, setStreetAddress] = useState(props.userInfo.street_addr);
    const [zipCode, setZipCode] = useState(props.userInfo.zip);
    const [userId, setUserId] = useState(props.userInfo.user_id);

    // First Name 
        // State variables
        const [newFirstName, setNewFirstName] = useState("");
        const [firstNameResponse, setFirstNameResponse] = useState("");
        // CSS variables
        const [firstNameOpen, setFirstNameOpen] = useState("flex");
        const [editFirstNameOpen, setEditFirstNameOpen] = useState("none");
        const [editFirstNameText, setEditFirstNameText] = useState("Edit");
        const [firstNameResponseColor, setFirstNameResponseColor] = useState("black");
    // Last Name 
        // State variables 
        const [newLastName, setNewLastName] = useState("");
        const [lastNameResponse, setLastNameResponse] = useState("");
        // CSS variables
        const [lastNameOpen, setLastNameOpen] = useState("flex");
        const [editLastNameOpen, setEditLastNameOpen] = useState("none");
        const [editLastNameText, setEditLastNameText] = useState("Edit");  
        const [lastNameResponseColor, setLastNameResponseColor] = useState("black");
    // Email Address 
        // State variables 
        const [newEmail, setNewEmail] = useState("");
        const [emailResponse, setEmailResponse] = useState("");
        // CSS variables
        const [emailOpen, setEmailOpen] = useState("flex");
        const [editEmailOpen, setEditEmailOpen] = useState("none");
        const [editEmailText, setEditEmailText] = useState("Edit");  
        const [emailResponseColor, setEmailResponseColor] = useState("black");
    // Password 
        // State variables
        const [oldPassword, setOldPassword] = useState("");
        const [newPassword, setNewPassword] = useState("");
        const [confirmPassword, setConfirmPassword] = useState("");
        const [passwordResponse, setPasswordResponse] = useState("");
        // CSS variables
        const [passwordOpen, setPasswordOpen] = useState("flex");
        const [editPasswordOpen, setEditPasswordOpen] = useState("none");
        const [editPasswordText, setEditPasswordText] = useState("Edit");  
        const [passwordResponseColor, setPasswordResponseColor] = useState("black");
    // Street Address 
        // State variables
        const [newStreetAddress, setNewStreetAddress] = useState("");
        const [streetAddressResponse, setStreetAddressResponse] = useState("");
        // CSS variables
        const [streetAddressOpen, setStreetAddressOpen] = useState("flex");
        const [editStreetAddressOpen, setEditStreetAddressOpen] = useState("none");
        const [editStreetAddressText, setEditStreetAddressText] = useState("Edit");
        const [streetAddressResponseColor, setStreetAddressResponseColor] = useState("black");
    // Zip Code 
        // State variables
        const [newZipCode, setNewZipCode] = useState("");
        const [zipCodeResponse, setZipCodeResponse] = useState("");
        // CSS variables
        const [zipCodeOpen, setZipCodeOpen] = useState("flex");
        const [editZipCodeOpen, setEditZipCodeOpen] = useState("none");
        const [editZipCodeText, setEditZipCodeText] = useState("Edit");  
        const [zipCodeResponseColor, setZipCodeResponseColor] = useState("black");


    /* logic functions ************************************************************************************************/

    // First Name Edit button function
    const HandleEditFirstName = (e) => {
        e.preventDefault();

        // Change text (Edit or Cancel)
        if(editFirstNameText === "Edit") setEditFirstNameText("Cancel");
        else setEditFirstNameText("Edit");

        // Toggle between "Change First Name form" or "Original First Name"
        if (editFirstNameOpen === "none") 
        {
            // Open form
            setEditFirstNameOpen("flex"); 
            setFirstNameOpen("none");
            setFirstNameResponse("");
            setFirstNameResponseColor("black");
        } 
        else 
        {
            // Close form and Clear passwords
            setEditFirstNameOpen("none");
            setFirstNameResponse("");
            setFirstNameResponseColor("darkgreen");
            setNewFirstName("");
            setFirstNameOpen("flex");
        }
    }
    // Last Name Edit button function
    const HandleEditLastName = (e) => {
        e.preventDefault();

        // Change text (Edit or Cancel)
        if(editLastNameText === "Edit") setEditLastNameText("Cancel");
        else setEditLastNameText("Edit");

        // Toggle between "Change Last Name form" or "Original Last Name"
        if (editLastNameOpen === "none") 
        {
            // Open form
            setEditLastNameOpen("flex"); 
            setLastNameOpen("none");
            setLastNameResponse("");
            setLastNameResponseColor("black");
        } 
        else 
        {
            // Close form and Clear passwords
            setEditLastNameOpen("none");
            setLastNameResponse("");
            setLastNameResponseColor("darkgreen");
            setNewLastName("");
            setLastNameOpen("flex");
        }
    }
    // Email Edit button function
    const HandleEditEmail = (e) => {
        e.preventDefault();

        // Change text (Edit or Cancel)
        if(editEmailText === "Edit") setEditEmailText("Cancel");
        else setEditEmailText("Edit");

        // Toggle between "Change Email form" or "Original Email"
        if (editEmailOpen === "none") 
        {
            // Open form
            setEditEmailOpen("flex"); 
            setEmailOpen("none");
            setEmailResponse("");
            setEmailResponseColor("black");
        } 
        else 
        {
            // Close form and Clear passwords
            setEditEmailOpen("none");
            setEmailResponse("");
            setEmailResponseColor("darkgreen");
            setNewEmail("");
            setEmailOpen("flex");
        }
    }
    // Password Edit button function
    const HandleEditPassword = (e) => {
        e.preventDefault();

        // Change text (Edit or Cancel)
        if(editPasswordText === "Edit") setEditPasswordText("Cancel");
        else setEditPasswordText("Edit");

        // Toggle between "Change Password form" or "Original Password text"
        if (editPasswordOpen === "none") 
        {
            // Open form
            setEditPasswordOpen("flex"); 
            setPasswordOpen("none");
            setPasswordResponse("");
            setPasswordResponseColor("black");
        } 
        else 
        {
            // Close form and Clear passwords
            setEditPasswordOpen("none");
            setPasswordResponse("");
            setPasswordResponseColor("darkgreen");
            setOldPassword("");
            setNewPassword("");
            setConfirmPassword("");
            setPasswordOpen("flex");
        }
    }
    // Street Address Edit button function
    const HandleEditStreetAddress = (e) => {
        e.preventDefault();

        // Change text (Edit or Cancel)
        if(editStreetAddressText === "Edit") setEditStreetAddressText("Cancel");
        else setEditStreetAddressText("Edit");

        // Toggle between "Change Street Address form" or "Original Street Address"
        if (editStreetAddressOpen === "none") 
        {
            // Open form
            setEditStreetAddressOpen("flex"); 
            setStreetAddressOpen("none");
            setStreetAddressResponse("");
            setStreetAddressResponseColor("black");
        } 
        else 
        {
            // Close form and Clear passwords
            setEditStreetAddressOpen("none");
            setStreetAddressResponse("");
            setStreetAddressResponseColor("darkgreen");
            setNewStreetAddress("");
            setStreetAddressOpen("flex");
        }
    }
    // Zip Code Edit button function
    const HandleEditZipCode = (e) => {
        e.preventDefault();

        // Change text (Edit or Cancel)
        if(editZipCodeText === "Edit") setEditZipCodeText("Cancel");
        else setEditZipCodeText("Edit");

        // Toggle between "Change Zip Code form" or "Original Zip Code"
        if (editZipCodeOpen === "none") 
        {
            // Open form
            setEditZipCodeOpen("flex"); 
            setZipCodeOpen("none");
            setZipCodeResponse("");
            setZipCodeResponseColor("black");
        } 
        else 
        {
            // Close form and Clear passwords
            setEditZipCodeOpen("none");
            setZipCodeResponse("");
            setZipCodeResponseColor("darkgreen");
            setNewZipCode("");
            setZipCodeOpen("flex");
        }
    }


    /* API function calls*****************************************************************************/

    // First Name Submit function
    const HandleSubmitFirstName = async (e) => {
        e.preventDefault();

        // Check if input is empty
        if (newFirstName === "") {
            setFirstNameResponse("Please enter first name");
            setFirstNameResponseColor("red");
            return;
        }
        // Try API call changeInfo
        try {
            await axiosJWT.post("/users/changeInfo", {
                userId: userId,
                userInfoColumn: "firstname",
                newInfo: newFirstName
            })
            .then(res => {
                // console.log(res); 
                // console.log(res.data);
                // Set response and response color
                if (res.data === "user info changed") 
                {
                    setFirstNameResponse("Succces, first name changed");
                    setFirstNameResponseColor("green");
                    setEditFirstNameOpen("none");
                    setFirstNameOpen("flex");
                    setEditFirstNameText("Edit");
                    setNewFirstName("");
                    setFirstName(newFirstName);
                } 
                else if (res.data === "user not found") 
                {
                    setFirstNameResponse("Contact support, user_id not found");
                    setFirstNameResponseColor("red");
                }
                else 
                {
                    setFirstNameResponse(res.data);
                    setFirstNameResponseColor("red");
                }
            })  
        }
        catch(err) {
            // console.log(err.message);
        }    
    };
    // Last Name Submit function
    const HandleSubmitLastName = async (e) => {
        e.preventDefault();

        // Check if input is empty
        if (newLastName === "") {
            setLastNameResponse("Please enter last name");
            setLastNameResponseColor("red");
            return;
        }
        // Try API call changeInfo
        try {
            await axiosJWT.post("/users/changeInfo", {
                userId: userId,
                userInfoColumn: "lastname",
                newInfo: newLastName
            })
            .then(res => {
                // console.log(res); 
                // console.log(res.data);
                // Set response and response color
                if (res.data === "user info changed") 
                {
                    setLastNameResponse("Succces, last name changed");
                    setLastNameResponseColor("green");
                    setEditLastNameOpen("none");
                    setLastNameOpen("flex");
                    setEditLastNameText("Edit");
                    setNewLastName("");
                    setLastName(newLastName);
                } 
                else if (res.data === "user not found") 
                {
                    setLastNameResponse("Contact support, user_id not found");
                    setLastNameResponseColor("red");
                }
                else 
                {
                    setLastNameResponse(res.data);
                    setLastNameResponseColor("red");
                }
            })  
        }
        catch(err) {
            console.log(err.message);
        }    
    };
    // Email Submit function
    const HandleSubmitEmail = async (e) => {
        e.preventDefault();

        // Check if input is empty
        if (newEmail === "") {
            setEmailResponse("Please enter email");
            setEmailResponseColor("red");
            return;
        }
        // Try API call changeInfo
        try {
            await axiosJWT.post("/users/changeInfo", {
                userId: userId,
                userInfoColumn: "email",
                newInfo: newEmail
            })
            .then(res => {
                // console.log(res); 
                // console.log(res.data);
                // Set response and response color
                if (res.data === "user info changed") 
                {
                    setEmailResponse("Succces, email changed");
                    setEmailResponseColor("green");
                    setEditEmailOpen("none");
                    setEmailOpen("flex");
                    setEditEmailText("Edit");
                    setNewEmail("");
                    setEmail(newEmail);
                } 
                else if (res.data === "user not found") 
                {
                    setEmailResponse("Contact support, user_id not found");
                    setEmailResponseColor("red");
                }
                else 
                {
                    setEmailResponse(res.data);
                    setEmailResponseColor("red");
                }
            })  
        }
        catch(err) {
            console.log(err.message);
        }    
    };
    // Password Submit function
    const HandleSubmitPassword = async (e) => {
        e.preventDefault();

        // Check if input is empty
        if (oldPassword ==="" || newPassword ==="" || confirmPassword === "") {
            setPasswordResponse("Please enter password");
            setPasswordResponseColor("red");
            return;
        }
        // Check if new passwords don't match
        if (newPassword !== confirmPassword) {
            setPasswordResponse("New passwords must match");
            setPasswordResponseColor("red");
            return;
        }
        // Try API call changePassword
        try {
            await axiosJWT.post("/users/changePassword", {
                userId: userId,
                oldPassword: oldPassword, 
                newPassword: newPassword
            })
            .then(res => {
                // console.log(res); 
                // console.log(res.data);
                // Set response and response color
                if (res.data === "password changed") 
                {
                    setPasswordResponse("Succces, password changed");
                    setPasswordResponseColor("green");
                    setEditPasswordOpen("none");
                    setPasswordOpen("flex");
                    setEditPasswordText("Edit");
                    setOldPassword("");
                    setNewPassword("");
                    setConfirmPassword("");
                } 
                else if (res.data === "password not found") 
                {
                    setPasswordResponse("Try again, password incorrect");
                    setPasswordResponseColor("red");
                }
                else 
                {
                    setPasswordResponse("Network error");
                    setPasswordResponseColor("red");
                }
            })  
        }
        catch(err) {
            console.log(err.message);
        }    
    };
    // Street Address Submit function
    const HandleSubmitStreetAddress = async (e) => {
        e.preventDefault();

        // Check if input is empty
        if (newStreetAddress === "") {
            setStreetAddressResponse("Please enter street address");
            setStreetAddressResponseColor("red");
            return;
        }
        // Try API call changeInfo
        try {
            await axiosJWT.post("/users/changeInfo", {
                userId: userId,
                userInfoColumn: "street_addr",
                newInfo: newStreetAddress
            })
            .then(res => {
                // console.log(res); 
                // console.log(res.data);
                // Set response and response color
                if (res.data === "user info changed") 
                {
                    setStreetAddressResponse("Succces, street address changed");
                    setStreetAddressResponseColor("green");
                    setEditStreetAddressOpen("none");
                    setStreetAddressOpen("flex");
                    setEditStreetAddressText("Edit");
                    setNewStreetAddress("");
                    setStreetAddress(newStreetAddress);
                } 
                else if (res.data === "user not found") 
                {
                    setStreetAddressResponse("Contact support, user_id not found");
                    setStreetAddressResponseColor("red");
                }
                else 
                {
                    setStreetAddressResponse(res.data);
                    setStreetAddressResponseColor("red");
                }
            })  
        }
        catch(err) {
            console.log(err.message);
        }    
    };
    // Zip Code Submit function
    const HandleSubmitZipCode = async (e) => {
        e.preventDefault();

        // Check if input is empty
        if (newZipCode === "") {
            setZipCodeResponse("Please enter zip code");
            setZipCodeResponseColor("red");
            return;
        }
        // Try API call changeInfo
        try {
            await axiosJWT.post("/users/changeInfo", {
                userId: userId,
                userInfoColumn: "zip",
                newInfo: newZipCode
            })
            .then(res => {
                // console.log(res); 
                // console.log(res.data);
                // Set response and response color
                if (res.data === "user info changed") 
                {
                    setZipCodeResponse("Succces, zip code changed");
                    setZipCodeResponseColor("green");
                    setEditZipCodeOpen("none");
                    setZipCodeOpen("flex");
                    setEditZipCodeText("Edit");
                    setNewZipCode("");
                    setZipCode(newZipCode);
                } 
                else if (res.data === "user not found") 
                {
                    setZipCodeResponse("Contact support, user_id not found");
                    setZipCodeResponseColor("red");
                }
                else 
                {
                    setZipCodeResponse(res.data);
                    setZipCodeResponseColor("red");
                }
            })  
        }
        catch(err) {
            console.log(err.message);
        }    
    };
    

    // CSS 
    return(
        <div>
            <div className= "fdAccountContainer">
                <div className= "fdTitleBar">
                    Profile Information
                </div>
                <div className= "fdAccountInfoWrapper">
                    {/* First Name Info */}
                    <div className= "fdAccountInfoItem">
                        <div className = "fdButtonContainer">
                            <button className = "fdButton" type="submit" onClick={HandleEditFirstName}>
                                <div className = "fdButtonText">
                                    {editFirstNameText}
                                </div>
                            </button>
                        </div>
                        <div style={{display: firstNameOpen}}>
                            <div className= "fdAccountInfoItemText">First Name:</div>
                            <div className= "fdAccountInfoItemText">{firstName}</div>
                        </div>
                        <div style={{display: editFirstNameOpen}}>
                            <div style={{flexDirection: "column", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChanged">
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: firstNameResponseColor}}
                                        type="text" 
                                        name="newFirstName" 
                                        placeholder="First Name"
                                        value= {newFirstName}
                                        onChange={(e) => {
                                            setNewFirstName(e.target.value);
                                        }}
                                    />
                                </div>
                                <div className= "fdAccountInfoItemChangedInstructions">  </div>
                            </div>
                            <div style={{display: "flex", flexDirection: "row", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChangedBtn">
                                    <button className= "fdButton2" type="submit" onClick={HandleSubmitFirstName}>
                                        <div className = "fdButtonText2">
                                            Change First Name
                                        </div>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className= "fdAccountInfoItemChangedInstructions" style={{color: firstNameResponseColor}}>
                            {firstNameResponse}
                        </div>
                    </div>
                    {/* Last Name Info */}
                    <div className= "fdAccountInfoItem">
                        <div className = "fdButtonContainer">
                            <button className = "fdButton" type="submit" onClick={HandleEditLastName}>
                                <div className = "fdButtonText">
                                    {editLastNameText}
                                </div>
                            </button>
                        </div>
                        <div style={{display: lastNameOpen}}>
                            <div className= "fdAccountInfoItemText">Last Name:</div>
                            <div className= "fdAccountInfoItemText">{lastName}</div>
                        </div>
                        <div style={{display: editLastNameOpen}}>
                            <div style={{flexDirection: "column", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChanged">
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: lastNameResponseColor}}
                                        type="text" 
                                        name="newLastName" 
                                        placeholder="Last Name"
                                        value= {newLastName}
                                        onChange={(e) => {
                                            setNewLastName(e.target.value);
                                        }}
                                    />
                                </div>
                                <div className= "fdAccountInfoItemChangedInstructions">  </div>
                            </div>
                            <div style={{display: "flex", flexDirection: "row", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChangedBtn">
                                    <button className= "fdButton2" type="submit" onClick={HandleSubmitLastName}>
                                        <div className = "fdButtonText2">
                                            Change Last Name
                                        </div>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className= "fdAccountInfoItemChangedInstructions" style={{color: lastNameResponseColor}}>
                            {lastNameResponse}
                        </div>
                    </div>
                    {/* Email Address Info */}
                    <div className= "fdAccountInfoItem">
                        <div className = "fdButtonContainer">
                            <button className = "fdButton" type="submit" onClick={HandleEditEmail}>
                                <div className = "fdButtonText">
                                    {editEmailText}
                                </div>
                            </button>
                        </div>
                        <div style={{display: emailOpen}}>
                            <div className= "fdAccountInfoItemText">Email Address:</div>
                            <div className= "fdAccountInfoItemText">{email}</div>
                        </div>
                        <div style={{display: editEmailOpen}}>
                            <div style={{flexDirection: "column", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChanged">
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: emailResponseColor}}
                                        type="text" 
                                        name="newEmail" 
                                        placeholder="Email Address"
                                        value= {newEmail}
                                        onChange={(e) => {
                                            setNewEmail(e.target.value);
                                        }}
                                    />
                                </div>
                                <div className= "fdAccountInfoItemChangedInstructions">  </div>
                            </div>
                            <div style={{display: "flex", flexDirection: "row", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChangedBtn">
                                    <button className= "fdButton2" type="submit" onClick={HandleSubmitEmail}>
                                        <div className = "fdButtonText2">
                                            Change Email Address
                                        </div>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className= "fdAccountInfoItemChangedInstructions" style={{color: emailResponseColor}}>
                            {emailResponse}
                        </div>
                    </div>
                    {/* Password Info */}
                    <div className= "fdAccountInfoItem">
                        <div className = "fdButtonContainer">
                            <button className = "fdButton" type="submit" onClick={HandleEditPassword}>
                                <div className = "fdButtonText">
                                    {editPasswordText}
                                </div>
                            </button>
                        </div>
                        <div style={{display: passwordOpen}}>
                            <div className= "fdAccountInfoItemText">Password:</div>
                            <div className= "fdAccountInfoItemText">********</div>
                        </div>
                        <div style={{display: editPasswordOpen}}>
                            <div style={{flexDirection: "column", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChanged">
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: passwordResponseColor}}
                                        type="text" 
                                        name="oldPassword" 
                                        placeholder="Old Password"
                                        value= {oldPassword}
                                        onChange={(e) => {
                                            setOldPassword(e.target.value);
                                        }}
                                    />
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: passwordResponseColor}}
                                        type="text" 
                                        name="newPassword"
                                        placeholder="New Password"
                                        value= {newPassword}
                                        onChange={(e) => {
                                            setNewPassword(e.target.value);
                                        }}
                                    />
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: passwordResponseColor}}
                                        type="text" 
                                        name="newPassword"
                                        placeholder="Confirm Password"
                                        value= {confirmPassword}
                                        onChange={(e) => {
                                            setConfirmPassword(e.target.value);
                                        }}
                                    />
                                </div>
                                <div className= "fdAccountInfoItemChangedInstructions"> *Password must be atleast 6 characters including atleast one number and letter* </div>
                            </div>
                            <div style={{display: "flex", flexDirection: "row", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChangedBtn">
                                    <button className= "fdButton2" type="submit" onClick={HandleSubmitPassword}>
                                        <div className = "fdButtonText2">
                                            Change Password
                                        </div>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className= "fdAccountInfoItemChangedInstructions" style={{color: passwordResponseColor}}>
                            {passwordResponse}
                        </div>
                    </div>
                    {/* Street Address Info */}
                    <div className= "fdAccountInfoItem">
                        <div className = "fdButtonContainer">
                            <button className = "fdButton" type="submit" onClick={HandleEditStreetAddress}>
                                <div className = "fdButtonText">
                                    {editStreetAddressText}
                                </div>
                            </button>
                        </div>
                        <div style={{display: streetAddressOpen}}>
                            <div className= "fdAccountInfoItemText">Street Address:</div>
                            <div className= "fdAccountInfoItemText">{streetAddress}</div>
                        </div>
                        <div style={{display: editStreetAddressOpen}}>
                            <div style={{flexDirection: "column", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChanged">
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: streetAddressResponseColor}}
                                        type="text" 
                                        name="newStreetAddress" 
                                        placeholder="Street Address"
                                        value= {newStreetAddress}
                                        onChange={(e) => {
                                            setNewStreetAddress(e.target.value);
                                        }}
                                    />
                                </div>
                                <div className= "fdAccountInfoItemChangedInstructions">  </div>
                            </div>
                            <div style={{display: "flex", flexDirection: "row", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChangedBtn">
                                    <button className= "fdButton2" type="submit" onClick={HandleSubmitStreetAddress}>
                                        <div className = "fdButtonText2">
                                            Change Street Address
                                        </div>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className= "fdAccountInfoItemChangedInstructions" style={{color: streetAddressResponseColor}}>
                            {streetAddressResponse}
                        </div>
                    </div>
                    {/* Zip Code Info */}
                    <div className= "fdAccountInfoItem">
                        <div className = "fdButtonContainer">
                            <button className = "fdButton" type="submit" onClick={HandleEditZipCode}>
                                <div className = "fdButtonText">
                                    {editZipCodeText}
                                </div>
                            </button>
                        </div>
                        <div style={{display: zipCodeOpen}}>
                            <div className= "fdAccountInfoItemText">Zip Code:</div>
                            <div className= "fdAccountInfoItemText">{zipCode}</div>
                        </div>
                        <div style={{display: editZipCodeOpen}}>
                            <div style={{flexDirection: "column", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChanged">
                                    <input className= "fdAccountInfoItemChangedInput" style={{borderColor: zipCodeResponseColor}}
                                        type="text" 
                                        name="newZipCode" 
                                        placeholder="Zip Code"
                                        value= {newZipCode}
                                        onChange={(e) => {
                                            setNewZipCode(e.target.value);
                                        }}
                                    />
                                </div>
                                <div className= "fdAccountInfoItemChangedInstructions">  </div>
                            </div>
                            <div style={{display: "flex", flexDirection: "row", alignItems: "flex-start"}}>
                                <div className= "fdAccountInfoItemChangedBtn">
                                    <button className= "fdButton2" type="submit" onClick={HandleSubmitZipCode}>
                                        <div className = "fdButtonText2">
                                            Change Zip Code
                                        </div> 
                                    </button>
                                </div>
                            </div>
                        </div>
                        <div className= "fdAccountInfoItemChangedInstructions" style={{color: zipCodeResponseColor}}>
                            {zipCodeResponse}
                        </div>
                    </div>
                </div>            
                <div className= "fdTitleBar">
                    Subscription Status
                </div>
                <div className= "fdAccountInfoWrapper">
                    <div className= "fdAccountInfoItem">
                        Current Subscription:  30 Day Monthly
                    </div>
                    <div className= "fdAccountInfoItem">
                        Monthly Charge:  $10.99
                    </div>
                    <div className= "fdAccountInfoItem">
                        Next Billing Date: October 30th 2021 
                    </div>
                    <div className= "fdAccountInfoItem">
                        Expiration Date: N/A
                    </div>
                    <div className = "fdButtonContainer">
                        <div className = "fdButton">
                            <div className = "fdButtonText">
                                Change
                            </div>
                        </div>
                        <div className = "fdButton">
                            <div className = "fdButtonText">
                                Cancel
                            </div>
                        </div>
                    </div>
                </div>
                <div className= "fdTitleBar">
                    Payment Information
                </div>
                <div className= "fdAccountInfoWrapper">
                    <div className= "fdAccountInfoItem">
                        Current Payment Type:  Visa
                    </div>
                    <div className= "fdAccountInfoItem">
                        Last 4 Digits of Card:  ****4498
                    </div>
                    <div className= "fdAccountInfoItem">
                        Card Expiration: 9/23
                    </div>
                    <div className = "fdButtonContainer">
                        <div className = "fdButton">
                            <div className = "fdButtonText">
                                Edit
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>   
    )

}

export default withRouter(AccountSettings);