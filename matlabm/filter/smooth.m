%Savitzky-Golay ƽ������;Ĭ�ϵĶ���ʽ����Ϊ3
%x:����Ⱦ���;
%m:ƽ���������������5;
function xc=smooth(x,wp)
	k=3;
	xc=sgolayfilt(x,k,wp);
end
