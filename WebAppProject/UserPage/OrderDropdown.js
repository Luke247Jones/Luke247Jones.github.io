import React from 'react';
import axios from "axios";
import {useState, useEffect} from 'react';
import '../../Components/FormDropdown.css';
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';
import { FiChevronDown } from "react-icons/fi";
import { FiChevronUp } from "react-icons/fi";
import IconButton from '@mui/material/IconButton';
import OrderHistoryItem from './OrderHistoryItem';
import UpcomingPickupsItem from './UpcomingPickupsItem';

const OrderDropdown = (props) => {
    const [open, setOpen] = useState(false);
    const [orderColor, setOrderColor] = useState("white");
    const [statusColor, setStatusColor] = useState("green");

    useEffect(() => {
        if (props.title === "Upcoming Pickups") setOrderColor("#DBEBE7");
        else setOrderColor("#DBEBE7");
        if (props.orderInfoItem.order_status === "Cancelled") setStatusColor("red");
        if (props.orderInfoItem.order_status !== "Delivered") setStatusColor("orange");
    }, []);
    

    if (open === true) {
        return <OrderDropdownOpen/>;
    }
    else {
        return <OrderDropdownClosed/>;
    } 

    function OrderDropdownOpen() {
        return (
            <div className = "orderDropdownContainer">
                <header className = "orderDropdownTitle" style={{backgroundColor: orderColor}}>
                    <div className = "orderDropdownTitleText">
                        {props.orderInfoItem.order_date}
                    </div>
                    <div className = "orderDropdownTitleText">
                        <div> Status: </div>
                        <div className= "orderDropdownTitleTextComplete" style={{color: statusColor}}>{props.orderInfoItem.order_status}</div>                
                    </div>
                    <div className = "orderDropdownTitleText">
                        <IconButton onClick = {() => {setOpen(!open)}}>
                            <FiChevronUp></FiChevronUp>
                        </IconButton> 
                    </div>
                </header>
                <OrderDropdownContent></OrderDropdownContent>
            </div>
        )
    }

    function OrderDropdownClosed() {
    
        return (
            <div className = "orderDropdownContainer">
                <header className = "orderDropdownTitle" style={{backgroundColor: orderColor}}>
                    <div className = "orderDropdownTitleText">
                        {props.orderInfoItem.order_date}
                    </div>
                    <div className = "orderDropdownTitleText">
                        <div> Status: </div>
                        <div className= "orderDropdownTitleTextComplete" style={{color: statusColor}}>{props.orderInfoItem.order_status}</div>                
                    </div>
                    <div className = "orderDropdownTitleText">
                        <IconButton onClick = {() => {setOpen(!open)}}>
                            <FiChevronDown></FiChevronDown>
                        </IconButton> 
                    </div> 
                </header>
            </div>
        )
    }

    function OrderDropdownContent() {
        if (props.title === "Order History") return <OrderHistoryItem userInfo = {props.userInfo} orderInfoItem = {props.orderInfoItem} boxInfoItem = {props.boxInfoItem}></OrderHistoryItem>
        if (props.title === "Upcoming Pickups") return <UpcomingPickupsItem userInfo = {props.userInfo} orderInfoItem = {props.orderInfoItem} boxInfoItem = {props.boxInfoItem}></UpcomingPickupsItem>
    }

}


export default OrderDropdown;