import React, {useEffect} from 'react';
import axios from "axios";
import jwtDecode from 'jwt-decode';
import { useHistory, Link, withRouter } from 'react-router-dom';
import Calendar from 'react-calendar';
import '../../Components/FormDropdown.css';
import OrderDropdown from './OrderDropdown';
import ReactGA from "react-ga";

ReactGA.initialize(process.env.GA_TRACKING_CODE);

const UpcomingPickups = (props) => {

    useEffect(() => {
        ReactGA.pageview(window.location.pathname + window.location.search);
    });

    const dataColumn = ["Date", "Time", "Address", "Size", "Notes", "Status"];
    const dataRow = ["10/28/21", "4:30pm", "123 John Doe Ln", "14 boxes", "Pick-up boxes near left garage door", "Scheduled"];
    const pickupDate = new Date(2021, 10 -1, 28);

    // CSS
    return (
        <div>
            <UpcomingPickupsContent></UpcomingPickupsContent>
        </div>
    )

    // Return array of Order History Items
    function UpcomingPickupsContent() {

        // Loop for returning React Components
        let renderedItemArray = [];
        console.log(props.orderInfoList);

        if (props.orderInfoList.length > 0) {
            for (let i = 0; i < props.orderInfoList.length; i++) {
                if (props.orderInfoList[i].order_status !== "Delivered" && props.orderInfoList[i].order_status !== "Cancelled") 
                {
                    renderedItemArray.push(<OrderDropdown title = {props.title} userInfo = {props.userInfo} orderInfoItem = {props.orderInfoList[i]} boxInfoItem = {props.boxInfoList[i]}></OrderDropdown>);
                } 
            }
        }
        
        // Output CSS
        if (props.apiResponse === "user not found") return (<div className= "orderDropdownContainer"> User orders not found. </div>);
        else if (renderedItemArray.length > 0) return (renderedItemArray); 
        else return (<div className= "orderDropdownContainer"> You have 0 upcoming orders... </div>);
        
    }

    /*return (
        <div>
            <div className = "fdContentContainer">
                <div className = "fdLeftContent">
                    <div>
                        {dataColumn.map((label, index) => (
                            <li className="fdList" key={index}>
                                <div className="fdListItem">
                                    <div className = "fdTextLabel">
                                        {dataColumn[index]} :
                                    </div>
                                    <div className = "fdTextData">
                                        {dataRow[index]}
                                    </div>   
                                </div>  
                            </li>
                        ))}
                    </div>
                </div>
                <div className = "fdRightContent">
                    <div className = "fdCalendarContainer">
                        <Calendar className = "react-calendar" minDate={pickupDate} maxDate={pickupDate} defaultValue = {pickupDate} ></Calendar>
                    </div>
                </div>
            </div>
            <div className= "fdButtonsContainer">
                <btn className= "fdButton">
                    <div className = "fdButtonText">
                        Reschedule Pickup
                    </div>
                </btn>
                <btn className= "fdButton">
                    <div className = "fdButtonText">
                        Cancel Pickup
                    </div>
                </btn>
            </div>
        </div>
    )*/
}

export default withRouter(UpcomingPickups);