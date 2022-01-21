import React from 'react';
import '../../Components/FormDropdown.css';


const OrderHistoryItem = (props) => {
    return(
        <div className= "orderDropdownItemContainer">
            <div className= "orderDropdownInfoWrapper">
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Order ID:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.order_id : "Not Loaded"}</div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Date:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.order_date : "Not Loaded"}</div>
                </div>
                <div className= "fdAccountInfoItem">
                    <div className= "fdAccountInfoItemText">Time:</div>
                    <div className= "fdAccountInfoItemText">{props.orderInfoItem !== null ? props.orderInfoItem.order_time : "Not Loaded"}</div>
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
            </div>            
        </div>
    )
}

export default OrderHistoryItem;