function c =Confidence(myTQ,TQ,mySQ,SQ)

    c = 0;
    if myTQ>TQ && mySQ>SQ
        c = 9*(myTQ-TQ)/(1-TQ)+90;
    elseif myTQ>TQ
        c = 9*(myTQ-TQ)/(1-TQ)+90;
    else
        c = 0.5*(9*(myTQ-TQ)/(1-TQ)+95+20*(mySQ-SQ)/(1-SQ)+80);
    end
    if c<10
        c = 10;
    end
end



 