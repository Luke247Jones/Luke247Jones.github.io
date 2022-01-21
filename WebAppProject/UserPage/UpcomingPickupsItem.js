import React from 'react';
import {useState, useEffect} from 'react';
import axios from "axios";
import jwtDecode from 'jwt-decode';
import { withRouter } from 'react-router-dom';
import Calendar from 'react-calendar';
import Select from 'react-select';
import Modal from 'react-modal';
import 'react-calendar/dist/Calendar.css';
import '../../Components/FormDropdown.css';
import useAuth from '../../hooks/useAuth';
import useFindUser from '../../hooks/useFindUser';


const UpcomingPickupsItem = (props) => {
    // Set today's date
    const today = new Date();
    const minDate = today;
    const maxDate = new Date();
    maxDate.setMonth(today.getMonth() + 3);
    if (maxDate.getMonth() > 12) maxDate.setMonth(maxDate.getMonth() - 12);
    // Initialize useFindUser
    const {axiosJWT} = useFindUser();

    // State variables logic
    const [time, setNewTime] = useState(props.orderInfoItem.order_time);
    const [date, setNewDate] = useState(convertedDate(props.orderInfoItem.order_date));
    //const [minDate, setMinDate] = useState(today);
    //const [maxDate, setMaxDate] = useState(convertedDate(props.orderInfoItem.order_date));
    

    // State variables CSS
    const [rescheduleOpen, setRescheduleOpen] = useState("none");
    const [originalOpen, setOriginalOpen] = useState("flex");
    const [modalIsOpen, setIsOpen] = useState(false);
    const [buttonDisabled, setButtonDisabled] = useState(false);
    const [buttonColor, setButtonColor] = useState("#93d50a");
    const [rescheduleButtonText, setRescheduleButtonText] = useState("Reschedule Pickup");
    const [reschedulePickupResponse, setReschedulePickupResponse] = useState("");
    const [reschedulePickupResponseColor, setReschedulePickupResponseColor] = useState("");

    // Set time options
    const timeOptions = [
        { value: '8:00-9:00am', label: '8:00-9:00am' },
        { value: '9:00-10:00am', label: '9:00-10:00am' },
        { value: '10:00-11:00am', label: '10:00-11:00am' },
        { value: '11:00-12:00pm', label: '11:00-12:00pm' },
        { value: '12:00-1:00pm', label: '12:00-1:00pm' },
        { value: '1:00-2:00pm', label: '1:00-2:00pm' },
        { value: '2:00-3:00pm', label: '2:00-3:00pm' },
        { value: '3:00-4:00pm', label: '3:00-4:00pm' },
        { value: '4:00-5:00pm', label: '4:00-5:00pm' },
        { value: '5:00-6:00pm', label: '5:00-6:00pm' }
    ]

    // Convert string to Date object
    function convertedDate(date) {
        let dateArray = date.split("/");
        let dateMonth = dateArray[0];
        let dateDay = dateArray[1];
        let dateYear = dateArray[2];

        return new Date(dateYear, dateMonth-1, dateDay);
    }
    // Modal functions
    function openModal() {
        setIsOpen(true);
    }
    function closeModal() {
        setIsOpen(false);
    }


    // Logic functions
    function HandleRescheduleButton(e) {
        if(rescheduleButtonText === "Reschedule Pickup") 
        {
            // CSS
            setRescheduleButtonText("Confirm Reschedule");
            setOriginalOpen("none");
            setRescheduleOpen("flex");
            setReschedulePickupResponse("");
        }
        else 
        {
            // CSS
            setRescheduleButtonText("Reschedule Pickup");
            setOriginalOpen("flex");
            setRescheduleOpen("none");

            // Reschedule API
            HandleReschedulePickup(e);
        }
    }
    function HandleCancelRescheduleButton() {
        setOriginalOpen("flex");
        setRescheduleOpen("none");
        setNewTime(props.orderInfoItem.order_time);
        setRescheduleButtonText("Reschedule Pickup");
    }

    // API functions
    const HandleCancelPickup = async (e) => {
        closeModal();
        e.preventDefault();

        // API call
        if (sessionStorage.getItem("accessToken") !== null) {
            try {
                axiosJWT.post("/users/changeOrderStatus", {
                    orderId: props.orderInfoItem.order_id,
                    orderStatus: "Cancelled"
                })
                .then(res => {
                    console.log(res); 
                    console.log(res.data);
                    // Set response and response color
                    if (res.data === "order status changed") 
                    {
                        setReschedulePickupResponse("Succces, pickup has been cancelled");
                        setReschedulePickupResponseColor("green");
                        setOriginalOpen("flex");
                        setRescheduleOpen("none");
                        setButtonDisabled(true);
                        setButtonColor("lightgrey")
                    } 
                    else if (res.data === "order not found") 
                    {
                        setReschedulePickupResponse("Try again or contact support: order could not be found");
                        setReschedulePickupResponseColor("red");
                    }
                    else 
                    {
                        setReschedulePickupResponse("Network error");
                        setReschedulePickupResponseColor("red");
                    }
                })  
            }
            catch(err) {
                console.log(err.message);
            }
        }
    }
    const HandleReschedulePickup = async (e) => {
        e.preventDefault();

        if (date === props.orderInfoItem.order_date || time === props.orderInfoItem.order_time) 
        {
            setReschedulePickupResponse("Please enter a new date or time.");
            setReschedulePickupResponseColor("red");
            return;
        }
        // API call
        if (sessionStorage.getItem("accessToken") !== null) {
            try {
                axiosJWT.post("/users/changeDateTime", {
                    orderId: props.orderInfoItem.order_id,
                    date: date.toISOString(), 
                    time: time
                })
                .then(res => {
                    console.log(res); 
                    console.log(res.data);
                    // Set response and response color
                    if (res.data === "date and time changed") 
                    {
                        setReschedulePickupResponse("Succces, pickup has been rescheduled");
                        setReschedulePickupResponseColor("green");
                        setOriginalOpen("flex");
                        setRescheduleOpen("none");
                    } 
                    else if (res.data === "order not found") 
                    {
                        setReschedulePickupResponse("Try again or contact support: order could not be found");
                        setReschedulePickupResponseColor("red");
                    }
                    else 
                    {
                        setReschedulePickupResponse("Network error");
                        setReschedulePickupResponseColor("red");
                    }
                })  
            }
            catch(err) {
                console.log(err.message);
            }
        }
    }

    return(
        <div className= "orderDropdownItemContainer">
            <div className= "orderDropdownInfoWrapper">
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Order ID:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.order_id : "Not Loaded"}</div>
                </div>
                <div className="upcomingPickupsDateTimeItem">
                    <div className = "fdButtonsContainer" style={{display: rescheduleOpen}}>
                        <button className = "upcomingPickupsButton1" type="submit" onClick={HandleCancelRescheduleButton}>
                            <div className = "fdButtonText">
                                Cancel
                            </div>
                        </button>
                    </div>
                    <div className= "upcomingPickupsDateTimeWrapper">
                        <div className= "fdAccountInfoItem">
                            <div className= "fdAccountInfoItemText">Date:</div>
                            <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? date.toDateString() : "Not Loaded"}</div> 
                        </div>
                        <div style={{display: rescheduleOpen}}>
                            <div className= "upcomingPickupsDateTimeInstructions">*Please enter your rescheduled date on calendar</div>
                        </div>
                        <div className= "fdAccountInfoItem">
                            <div className= "fdAccountInfoItemText">Time:</div>
                            <div style={{display: originalOpen}}>
                                <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? time : "Not Loaded"}</div>
                            </div>
                            <div style={{display: rescheduleOpen}}>
                                <div style={{width: "180px", height: "20px", marginLeft: "10px", marginBottom: "10px"}}>
                                    <Select  value={time} options={timeOptions} placeholder={time} onChange={(e) => {setNewTime(e.value)}}/>
                                </div>
                            </div>
                        </div>
                        <div style={{display: rescheduleOpen}}>
                            <div className= "upcomingPickupsDateTimeInstructions">*Please enter your rescheduled time</div>
                        </div>
                    </div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Address:</div>
                    <div className= "fdAccountInfoItemText">{props.userInfo !== null ? props.userInfo.street_addr : "Not Loaded"}</div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Size:</div>
                    <div className= "fdAccountInfoItemText">{props.boxInfoItem !== null ? props.boxInfoItem.quantity : "Not Loaded"}</div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Pickup Notes:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.pick_up_placement_note : "Not Loaded"}</div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Comments:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.order_comment : "Not Loaded"}</div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Payment:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.total : "Not Loaded"}</div> 
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Status:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.order_status : "Not Loaded"}</div> 
                </div>
                <div className = "upcomingPickupsButtonsContainer">
                    <div className = "upcomingPickupsButtonsContainer">
                        <button className = "upcomingPickupsButton2" type="submit" onClick={HandleRescheduleButton} disabled={buttonDisabled} style={{backgroundColor: buttonColor}}>
                            <div className = "fdButtonText">
                                {rescheduleButtonText}
                            </div>
                        </button>
                    </div>
                    <div className = "upcomingPickupsButtonsContainer">
                        <button className = "upcomingPickupsButton2" type="submit" onClick={openModal} disabled={buttonDisabled} style={{backgroundColor: buttonColor}}>
                            <div className = "fdButtonText">
                                Cancel Pickup
                            </div>
                        </button>
                    </div>
                </div>
                <div className= "upcomingPickupsResponse" style={{color: reschedulePickupResponseColor}}>
                    {reschedulePickupResponse}
                </div>
                <div>
                    <Modal
                        isOpen={modalIsOpen}
                        onRequestClose={closeModal}
                        contentLabel="Confirm Cancel Order"
                        style={{content: {
                            top: '50%',
                            left: '50%',
                            right: 'auto',
                            bottom: 'auto',
                            marginRight: '-50%',
                            transform: 'translate(-50%, -50%)',
                          }}}
                    >
                        <div className="orderDropdownInfoWrapper">
                            <h2>Are you sure you want to cancel your order?</h2>
                            <div className = "upcomingPickupsButtonsContainer">
                                <button className = "upcomingPickupsButton1" type="submit" onClick={HandleCancelPickup}>
                                    <div className = "fdButtonText">
                                        Yes
                                    </div>
                                </button>
                                <button className = "upcomingPickupsButton1" type="submit" onClick={closeModal}>
                                    <div className = "fdButtonText">
                                        No
                                    </div>
                                </button>
                            </div>
                        </div>
                    </Modal>
                </div>
            </div>
            <div className= "upcomingPickupsCalendarWrapper">
                <div style={{display: originalOpen}}>
                    {/*Current Calender*/}
                    <Calendar className = "react-calendar" onChange={(value, e) => {setNewDate(value)}} value={date} minDate={date} maxDate={date} defaultValue={date} ></Calendar>
                </div>
                <div style={{display: rescheduleOpen}}>
                    {/*Reschedule Calender*/}
                    <Calendar className = "react-calendar" onChange={(value, e) => {setNewDate(value)}} value={date} minDate={minDate} maxDate={maxDate} defaultValue={date} ></Calendar>
                </div>
            </div>           
        </div>
    )
}

export default UpcomingPickupsItem;